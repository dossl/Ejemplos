# -*- encoding: utf-8 -*-
##############################################################################
#
#    OpenERP, Open Source Management Solution
#    Copyright (C) 2004-2010 Tiny SPRL (<http://tiny.be>).
#
#    This program is free software: you can redistribute it and/or modify
#    it under the terms of the GNU Affero General Public License as
#    published by the Free Software Foundation, either version 3 of the
#    License, or (at your option) any later version.
#
#    This program is distributed in the hope that it will be useful,
#    but WITHOUT ANY WARRANTY; without even the implied warranty of
#    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#    GNU Affero General Public License for more details.
#
#    You should have received a copy of the GNU Affero General Public License
#    along with this program.  If not, see <http://www.gnu.org/licenses/>.
#
##############################################################################
import time
from datetime import datetime

from openerp.osv import osv
from openerp.report import report_sxw


class account_chart_element_report(report_sxw.rml_parse):
    def __init__(self, cr, uid, name, context):
        super(account_chart_element_report, self).__init__(cr, uid, name, context=context)
        self.context = context
        self.char_dict = {}
        self.period_totals = map(lambda x: 0, range(14))
        self.localcontext.update({
            'cr': cr,
            'uid': uid,
            'time': time,
            'summary': self._get_summary,
            'detail': self._get_detail,
            'subdetail': self._get_element_detail,
            'p_totals': self._get_period_totals
        })

    def set_context(self, objects, data, ids, report_type=None):
        return super(account_chart_element_report, self).set_context(objects, data, ids, report_type=report_type)

    def _get_root_accounts(self):
        account_ids = []
        chart_elements_pool = self.pool.get('account.chart.element')
        root_ids = chart_elements_pool.search(self.cr, self.uid, [('parent_id', '=', False)])
        for r in chart_elements_pool.browse(self.cr, self.uid, root_ids, context=self.context):
            account_ids.extend(ch.id for ch in r.account_ids)
            self.char_dict.update({r.code: r.name})
        return list(set(account_ids))

    def _get_period_totals(self):
        return self.period_totals

    def _get_summary(self, form):
        account_pool = self.pool.get('account.account')
        root_ids = self._get_root_accounts()
        valid_ids = set(form['ids']).intersection(
            self.pool.get('account.analytic.line').search(self.cr, self.uid, [('account_id', 'child_of', root_ids)]))
        in_process_ids = self._get_in_production_ids(form)
        summary = {}
        for aml in self.pool.get('account.analytic.line').browse(self.cr, self.uid, valid_ids, context=self.context):
            parent = aml.account_id.parent_id
            if aml.account_id.id in in_process_ids:
                if account_pool.search_count(self.cr, self.uid, [('code', '=', parent.code)]):
                    continue
            code = aml.account_id.code[-5:]
            element_code = code[:2]
            name = self.char_dict.get(element_code, 'Desconocido')
            date_obj = datetime.strptime(aml.date, '%Y-%m-%d')
            pos = date_obj.month
            amount = aml.amount
            if element_code in summary.keys():
                summary[element_code]['totals'][pos] += amount
            else:
                totals = map(lambda x: 0, range(14))
                totals[pos] = amount
                summary.update({element_code: {'totals': totals, 'name': '%s %s' % (element_code, name)}})
            summary[element_code]['totals'][-1] += amount
            self.period_totals[pos] += amount
            self.period_totals[-1] += amount
        return summary.values()

    def _get_in_production_ids(self, form):
        analytic_pool = self.pool.get('account.analytic.account')
        in_process_ids = analytic_pool.search(self.cr, self.uid, [('id', 'child_of', form['in_process_id'])],
                                              context=self.context)
        return in_process_ids

    def _get_detail(self, form):
        account_pool = self.pool.get('account.account')
        root_ids = self._get_root_accounts()
        valid_ids = set(form['ids']).intersection(
            self.pool.get('account.analytic.line').search(self.cr, self.uid, [('account_id', 'child_of', root_ids)]))
        detail = {}
        in_process_ids = self._get_in_production_ids(form)
        for aml in self.pool.get('account.analytic.line').browse(self.cr, self.uid, valid_ids, context=self.context):
            parent = aml.account_id.parent_id
            if aml.account_id.id in in_process_ids:
                if account_pool.search_count(self.cr, self.uid, [('code', '=', parent.code)]):
                    continue
                parent = parent.parent_id
            code = aml.account_id.code[-5:]
            element_code = code[:2]
            name = self.char_dict.get(element_code, 'Desconocido')
            date_obj = datetime.strptime(aml.date, '%Y-%m-%d')
            pos = date_obj.month
            amount = aml.amount
            if element_code in detail.keys():
                detail[element_code]['totals'][pos] += amount
                detail[element_code]['totals'][-1] += amount
                if parent.code in detail[element_code]['accounts']:
                    detail[element_code]['accounts'][parent.code]['subtotals'][pos] += amount
                else:
                    subtotals = map(lambda x: 0, range(14))
                    subtotals[pos] += amount
                    detail[element_code]['accounts'].update({
                        parent.code: {
                            'name': '%s %s' % (parent.code, parent.name),
                            'subtotals': subtotals
                        }
                    })
                detail[element_code]['accounts'][parent.code]['subtotals'][-1] += amount
            else:
                totals = map(lambda x: 0, range(14))
                totals[pos] += amount
                totals[-1] += amount
                subtotals = map(lambda x: 0, range(14))
                subtotals[pos] += amount
                subtotals[-1] += amount
                detail.update({element_code: {
                    'totals': totals,
                    'name': '%s %s' % (element_code, name),
                    'accounts': {
                        parent.code: {
                            'name': '%s %s' % (parent.code, parent.name),
                            'subtotals': subtotals
                        }
                    }}
                })
            self.period_totals[pos] += amount
            self.period_totals[-1] += amount
        return detail.values()

    def _get_element_detail(self, form):
        account_pool = self.pool.get('account.account')
        root_ids = self._get_root_accounts()
        valid_ids = set(form['ids']).intersection(
            self.pool.get('account.analytic.line').search(self.cr, self.uid, [('account_id', 'child_of', root_ids)]))
        detail = {}
        in_process_ids = self._get_in_production_ids(form)
        for aml in self.pool.get('account.analytic.line').browse(self.cr, self.uid, valid_ids, context=self.context):
            parent = aml.account_id.parent_id
            if aml.account_id.id in in_process_ids:
                if account_pool.search_count(self.cr, self.uid, [('code', '=', parent.code)]):
                    continue
                parent = parent.parent_id
            code = aml.account_id.code[-5:]
            element_code = code[:2]
            name = self.char_dict.get(element_code, 'Desconocido')
            date_obj = datetime.strptime(aml.date, '%Y-%m-%d')
            pos = date_obj.month
            amount = aml.amount
            if element_code in detail.keys():
                detail[element_code]['totals'][pos] += amount
                detail[element_code]['totals'][-1] += amount
                if code in detail[element_code]['detail'].keys():
                    detail[element_code]['detail'][code]['d_totals'][pos] += amount
                    detail[element_code]['detail'][code]['d_totals'][-1] += amount
                    if parent.code in detail[element_code]['detail'][code]['accounts'].keys():
                        detail[element_code]['detail'][code]['accounts'][parent.code]['subtotals'][pos] += amount
                    else:
                        subtotals = map(lambda x: 0, range(14))
                        subtotals[pos] += amount
                        detail[element_code]['detail'][code]['accounts'].update({
                            parent.code: {
                                'name': '%s %s' % (parent.code, parent.name),
                                'subtotals': subtotals
                            }
                        })
                    detail[element_code]['detail'][code]['accounts'][parent.code]['subtotals'][-1] += amount
                else:
                    p_ids = self.pool.get('account.chart.element').search(self.cr, self.uid,
                                                                          [('code', '=', element_code)],
                                                                          limit=1)
                    detail_ids = self.pool.get('account.chart.element').search(self.cr, self.uid,
                                                                               [('parent_id', '=', p_ids[0]),
                                                                                ('code', '=', code[-3:])],
                                                                               limit=1)
                    detail_name = self.pool.get('account.chart.element').browse(self.cr, self.uid, detail_ids[0]).name
                    d_totals = map(lambda x: 0, range(14))
                    d_totals[pos] += amount
                    d_totals[-1] += amount
                    subtotals = map(lambda x: 0, range(14))
                    subtotals[pos] += amount
                    subtotals[-1] += amount
                    detail[element_code]['detail'].update({
                        code: {
                            'name': '%s %s' % (code, detail_name),
                            'd_totals': d_totals,
                            'accounts': {
                                parent.code: {
                                    'name': '%s %s' % (parent.code, parent.name),
                                    'subtotals': subtotals
                                }
                            }
                        }})
            else:
                totals = map(lambda x: 0, range(14))
                totals[pos] += amount
                totals[-1] += amount
                d_totals = map(lambda x: 0, range(14))
                d_totals[pos] += amount
                d_totals[-1] += amount
                subtotals = map(lambda x: 0, range(14))
                subtotals[pos] += amount
                subtotals[-1] += amount
                p_ids = self.pool.get('account.chart.element').search(self.cr, self.uid, [('code', '=', element_code)],
                                                                      limit=1)
                detail_ids = self.pool.get('account.chart.element').search(self.cr, self.uid,
                                                                           [('parent_id', '=', p_ids[0]),
                                                                            ('code', '=', code[-3:])],
                                                                           limit=1)
                detail_name = self.pool.get('account.chart.element').browse(self.cr, self.uid, detail_ids[0]).name
                detail.update({element_code: {
                    'totals': totals,
                    'name': '%s %s' % (element_code, name),
                    'detail': {code: {
                        'name': '%s %s' % (code, detail_name),
                        'd_totals': d_totals,
                        'accounts': {
                            parent.code: {
                                'name': '%s %s' % (parent.code, parent.name),
                                'subtotals': subtotals
                            }
                        }
                    }}}})
            self.period_totals[pos] += amount
            self.period_totals[-1] += amount
        return detail.values()


class report_account_chart_element(osv.AbstractModel):
    _name = 'report.sale_mrp_account_ingenius_tsj.report_account_chart_element_tsj'
    _inherit = 'report.abstract_report'
    _template = 'sale_mrp_account_ingenius_tsj.report_account_chart_element_tsj'
    _wrapped_report_class = account_chart_element_report
