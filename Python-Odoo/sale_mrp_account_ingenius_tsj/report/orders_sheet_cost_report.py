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
from openerp.report import report_sxw
from openerp.tools.float_utils import float_round

import xlwt
from openerp.addons.report_xls.report_xls import report_xls


class orders_sheet_cost_report(report_sxw.rml_parse):
    def __init__(self, cr, uid, name, context):
        super(orders_sheet_cost_report, self).__init__(cr, uid, name, context=context)
        self.context = context
        self.char_dict = {}
        self.totals = {'si_total': 0, 'mp_pred_total': 0, 'mod_pred_total': 0, 'se_pred_total': 0, 'og_pred_total': 0,
                       'gi_pred_total': 0, 'mp_real_total': 0, 'mod_real_total': 0,
                       'se_real_total': 0, 'og_real_total': 0, 'gi_real_total': 0,
                       'mp_delta_total': 0, 'mod_delta_total': 0, 'se_delta_total': 0, 'og_delta_total': 0,
                       'gi_delta_total': 0, 'tot_pred': 0, 'tot_real': 0, 'tot_delta': 0,
                       'cant_prod_total': 0, 'cant_vend_total': 0, 'pre_vent_total': 0, 'val_vent_total': 0}
        self.localcontext.update({
            'cr': cr,
            'uid': uid,
            'time': time,
            'lines': self._get_data,
            'totals': self._get_totals
        })

    def _get_totals(self):
        return self.totals

    def _get_pred_vals(self, o):
        vals = {'mp_pred': 0, 'mod_pred': 0, 'se_pred': 0, 'og_pred': 0, 'gi_pred': 0, 'tot_pred': 0}
        for l in o.offer_line_ids:
            if l.expense_type == 'service':
                vals['se_pred'] += l.amount_total
                vals['tot_pred'] += l.amount_total
            elif l.expense_type == 'mp':
                vals['mp_pred'] += l.amount_total
                vals['tot_pred'] += l.amount_total
            elif l.expense_type == 'other':
                vals['og_pred'] += l.amount_total
                vals['tot_pred'] += l.amount_total
            elif l.expense_type == 'indirect':
                vals['gi_pred'] += l.amount_total
                vals['tot_pred'] += l.amount_total
            elif l.expense_type == 'direct':
                base = l.amount_untaxed
                for t in l.tax_ids:
                    amount = base + t.compute_taxes(amount=base, precision=8)
                    if t.expense_type == 'direct':
                        vals['mod_pred'] += amount
                        vals['tot_pred'] += amount
                    elif t.expense_type == 'indirect':
                        if not t.is_accumulative:
                            vals['gi_pred'] += amount
                            vals['tot_pred'] += amount 
                        else:    
                            vals['gi_pred'] += amount - base
                            vals['tot_pred'] += amount - base
        #vals['gi_pred']= vals['gi_pred']*0.83 #Ajusto imprecisión fija del calculo prueba
        self.totals['mp_pred_total'] += vals['mp_pred']
        self.totals['mod_pred_total'] += vals['mod_pred']
        self.totals['se_pred_total'] += vals['se_pred']
        self.totals['og_pred_total'] += vals['og_pred']
        self.totals['gi_pred_total'] += vals['gi_pred']
        self.totals['tot_pred'] += vals['tot_pred']
        return vals

    def _get_real_cost(self, a):
        vals = {'si': 0, 'mp_real': 0, 'mod_real': 0, 'se_real': 0, 'og_real': 0, 'gi_real': 0, 'tot_real': 0}
        elements = {'si': '00', 'mp': '10', 'mo': '50', 'se': '60', 'og': '80', 'gi': '90001'}

        valid_codes = elements.values()
        codes = map(lambda c: str((a.code or '') + c), valid_codes)
        ids = self.pool.get('account.analytic.account').search(self.cr, self.uid, [('code', 'in', codes)],
                                                               limit=len(codes))
        for aa in self.pool.get('account.analytic.account').browse(self.cr, self.uid, ids):
            amount = aa.debit
            vals['tot_real'] += amount
            if aa.code[-2:] == elements['si']:
                vals['si'] += amount
            if aa.code[-2:] == elements['mp']:
                vals['mp_real'] += amount
            if aa.code[-2:] == elements['mo']:
                vals['mod_real'] += amount
            if aa.code[-2:] == elements['se']:
                vals['se_real'] += amount
            if aa.code[-2:] == elements['og']:
                vals['og_real'] += amount
            elif aa.code[-5:] == elements['gi']:
                vals['gi_real'] += amount
        self.totals['si_total'] += vals['si']
        self.totals['mp_real_total'] += vals['mp_real']
        self.totals['mp_delta_total'] = (self.totals['mp_pred_total'] - self.totals['mp_real_total'])
        self.totals['mod_real_total'] += vals['mod_real']
        self.totals['mod_delta_total'] = (self.totals['mod_pred_total'] - self.totals['mod_real_total'])
        self.totals['se_real_total'] += vals['se_real']
        self.totals['se_delta_total'] = (self.totals['se_pred_total'] - self.totals['se_real_total'])
        self.totals['og_real_total'] += vals['og_real']
        self.totals['og_delta_total'] = (self.totals['og_pred_total'] - self.totals['og_real_total'])
        self.totals['gi_real_total'] += vals['gi_real']
        self.totals['gi_delta_total'] = (self.totals['gi_pred_total'] - self.totals['gi_real_total'])
        self.totals['tot_real'] += vals['tot_real']
        self.totals['tot_delta'] = (self.totals['tot_pred'] - self.totals['tot_real'])
        return vals

    def _get_sale_values(self, o):
        ir_line_pool = self.pool.get('tsj.stock.picking.out.line')
        vals = {'cant_prod': 0, 'cant_vend': 0, 'pre_vent': 0, 'val_vent': 0}
        ir_dom = [('work_order_id', '=', o.id), ('product_qty', '>', 0)]
        ir_line_ids = ir_line_pool.search(self.cr, self.uid, ir_dom, context=self.context)
        cant_prod = sum(
            [irl.product_qty for irl in ir_line_pool.browse(self.cr, self.uid, ir_line_ids, context=self.context)])
        vals['cant_prod'] += cant_prod

        vs_line_pool = self.pool.get('tsj.stock.picking.out.line')
        vs_dom = [('work_order_id', '=', o.id), ('invoice_line_id', '!=', False), ('product_qty', '>', 0)]
        vs_line_ids = vs_line_pool.search(self.cr, self.uid, vs_dom, context=self.context)
        cant_vend = val_vent = 0
        for vsl in vs_line_pool.browse(self.cr, self.uid, vs_line_ids, context=self.context):
            cant_vend += vsl.product_qty
            val_vent += vsl.invoice_line_id.price_subtotal

        vals['cant_vend'] = cant_vend
        vals['pre_vent'] = val_vent and float_round(val_vent/cant_vend, precision_digits=2) or 0
        vals['val_vent'] = val_vent

        self.totals['cant_prod_total'] += vals['cant_prod']
        self.totals['cant_vend_total'] += vals['cant_vend']
        self.totals['pre_vent_total'] += vals['pre_vent']
        self.totals['val_vent_total'] += vals['val_vent']
        return vals

    def _generate_row(self, a, o, po):
        pred_vals = self._get_pred_vals(o)
        real_vals = self._get_real_cost(a)
        sale_vals = self._get_sale_values(po)
        c_specs = {
            'ot': '%s %s' % (po.number, po.name),
            'si': real_vals['si'],
            'mp_pred': pred_vals['mp_pred'],
            'mp_real': real_vals['mp_real'],
            'mp_delta': pred_vals['mp_pred'] - real_vals['mp_real'],
            'mod_pred': pred_vals['mod_pred'],
            'mod_real': real_vals['mod_real'],
            'mod_delta': pred_vals['mod_pred'] - real_vals['mod_real'],
            'se_pred': pred_vals['se_pred'],
            'se_real': real_vals['se_real'],
            'se_delta': pred_vals['se_pred'] - real_vals['se_real'],
            'og_pred': pred_vals['og_pred'],
            'og_real': real_vals['og_real'],
            'og_delta': pred_vals['og_pred'] - real_vals['og_real'],
            'gi_pred': pred_vals['gi_pred'],
            'gi_real': real_vals['gi_real'],
            'gi_delta': pred_vals['gi_pred'] - real_vals['gi_real'],
            'tot_pred': pred_vals['tot_pred'],
            'tot_real': real_vals['tot_real'],
            'tot_delta': pred_vals['tot_pred'] - real_vals['tot_real'],
            'cant_prod': sale_vals['cant_prod'],
            'cant_vend': sale_vals['cant_vend'],
            'pre_vent': sale_vals['pre_vent'],
            'val_vent': sale_vals['val_vent']
        }
        return c_specs

    def _get_data(self, form):
        result = []
        work_order_pool = self.pool.get('tsj.work.order')
        wo_ids = form.get('ids', [])
        for order in work_order_pool.browse(self.cr, self.uid, wo_ids, context=self.context):
            account = order.analytic_account_id
            offer = order.sale_offer_id
            row = self._generate_row(account, offer, order)
            result.append(row)
        return result


class orders_sheet_cost_report_xls(report_xls):
    column_sizes = [10 for i in range(22)]

    def print_line(self, line, decimal_style=None, total_style=None):
        c_specs = [
            ('ot', 3, 0, 'text', line['ot']),
            ('si', 1, 0, 'number', line['si'], None, decimal_style),
            ('mp_pred', 1, 0, 'number', line['mp_pred'], None, decimal_style),
            ('mp_real', 1, 0, 'number', line['mp_real'], None, decimal_style),
            ('mp_delta', 1, 0, 'number', line['mp_delta'], None, decimal_style),
            ('mod_pred', 1, 0, 'number', line['mod_pred'], None, decimal_style),
            ('mod_real', 1, 0, 'number', line['mod_real'], None, decimal_style),
            ('mod_delta', 1, 0, 'number', line['mod_delta'], None, decimal_style),
            ('se_pred', 1, 0, 'number', line['se_pred'], None, decimal_style),
            ('se_real', 1, 0, 'number', line['se_real'], None, decimal_style),
            ('se_delta', 1, 0, 'number', line['se_delta'], None, decimal_style),
            ('og_pred', 1, 0, 'number', line['og_pred'], None, decimal_style),
            ('og_real', 1, 0, 'number', line['og_real'], None, decimal_style),
            ('og_delta', 1, 0, 'number', line['og_delta'], None, decimal_style),
            ('gi_pred', 1, 0, 'number', line['gi_pred'], None, decimal_style),
            ('gi_real', 1, 0, 'number', line['gi_real'], None, decimal_style),
            ('gi_delta', 1, 0, 'number', line['gi_delta'], None, decimal_style),
            ('tot_pred', 1, 0, 'number', line['tot_pred'], None, total_style),
            ('tot_real', 1, 0, 'number', line['tot_real'], None, total_style),
            ('tot_delta', 1, 0, 'number', line['tot_delta'], None, total_style),
            ('cant_prod', 1, 0, 'number', line['cant_prod'], None, total_style),
            ('cant_ven', 1, 0, 'number', line['cant_vend'], None, total_style),
            ('prec_ven', 1, 0, 'number', line['pre_vent'], None, total_style),
            ('val_ven', 1, 0, 'number', line['val_vent'], None, total_style)
        ]
        row_data = self.xls_row_template(c_specs, [x[0] for x in c_specs])
        return row_data

    def generate_xls_report(self, _p, _xs, data, objects, wb):
        sheet_name = u'Ficha de Costo'
        ws = wb.add_sheet(sheet_name)
        ws.panes_frozen = True
        ws.remove_splits = True
        ws.portrait = 0  # Landscape
        ws.fit_width_to_pages = 1
        row_pos = 0
        # set print header/footer
        ws.header_str = self.xls_headers['standard']
        ws.footer_str = self.xls_footers['standard']
        # Title
        report_name = u'Ficha de Costo de Órdenes de Producción'
        cell_style = xlwt.easyxf(_xs['xls_title'] + _xs['center'])
        c_specs = [('report_name', 18, 4, 'text', report_name)]
        row_data = self.xls_row_template(c_specs, ['report_name'])
        row_pos = self.xls_write_row(ws, row_pos, row_data, row_style=cell_style)

        # write empty row to define column sizes
        # row_pos = self.print_empty_row(ws, row_pos)

        # Header Table
        cell_format = _xs['bold'] + _xs['fill_blue'] + _xs['borders_all']
        cell_style_center = xlwt.easyxf(cell_format + _xs['center'])
        c_specs = [
            ('ot', 3, 0, 'text', None),
            ('si', 1, 0, 'text', u'S.Inicial'),
            ('mp', 3, 0, 'text', u'Materia Prima'),
            ('mod', 3, 0, 'text', u'Mano de Obra Directa'),
            ('se', 3, 0, 'text', u'Servicio Externo'),
            ('og', 3, 0, 'text', u'Otros Gastos'),
            ('gi', 3, 0, 'text', u'Gastos Indirectos'),
            ('ct', 3, 0, 'text', u'Costo Total'),
            ('cp', 1, 0, 'text', u'Cant. Producida'),
            ('vt', 3, 0, 'text', u'Ventas')
        ]
        row_data = self.xls_row_template(c_specs, [x[0] for x in c_specs])
        row_pos = self.xls_write_row(ws, row_pos, row_data, row_style=cell_style_center)

        # SubHeader Table
        cell_format = _xs['bold'] + _xs['fill_grey'] + _xs['borders_all']
        cell_style = xlwt.easyxf(cell_format)
        cell_style_center = xlwt.easyxf(cell_format + _xs['center'])
        cell_style_right = xlwt.easyxf(cell_format + _xs['right'])
        c_specs = [
            ('ot', 3, 10, 'text', u'Orden de Trabajo', None, cell_style),
            ('si', 1, 10, 'text', u'SI', None, cell_style_right),
            ('mp_pred', 1, 10, 'text', u'PRED', None, cell_style_right),
            ('mp_real', 1, 10, 'text', u'REAL', None, cell_style_right),
            ('mp_delta', 1, 10, 'text', u'VAR', None, cell_style_right),
            ('mod_pred', 1, 10, 'text', u'PRED', None, cell_style_right),
            ('mod_real', 1, 10, 'text', u'REAL', None, cell_style_right),
            ('mod_delta', 1, 10, 'text', u'VAR', None, cell_style_right),
            ('se_pred', 1, 10, 'text', u'PRED', None, cell_style_right),
            ('se_real', 1, 10, 'text', u'REAL', None, cell_style_right),
            ('se_delta', 1, 10, 'text', u'VAR', None, cell_style_right),
            ('og_pred', 1, 10, 'text', u'PRED', None, cell_style_right),
            ('og_real', 1, 10, 'text', u'REAL', None, cell_style_right),
            ('og_delta', 1, 10, 'text', u'VAR', None, cell_style_right),
            ('gi_pred', 1, 10, 'text', u'PRED', None, cell_style_right),
            ('gi_real', 1, 10, 'text', u'REAL', None, cell_style_right),
            ('gi_delta', 1, 10, 'text', u'VAR', None, cell_style_right),
            ('tot_pred', 1, 10, 'text', u'PRED', None, cell_style_right),
            ('tot_real', 1, 10, 'text', u'REAL', None, cell_style_right),
            ('tot_delta', 1, 10, 'text', u'VAR', None, cell_style_right),
            ('cant_prod', 1, 18, 'text', u'Cant. Prod', None, cell_style_right),
            ('cant_ven', 1, 15, 'text', u'Cant. Vendida', None, cell_style_right),
            ('prec_ven', 1, 10, 'text', u'P. Venta', None, cell_style_right),
            ('val_ven', 1, 10, 'text', u'V. Venta', None, cell_style_right)

        ]
        row_data = self.xls_row_template(c_specs, [x[0] for x in c_specs])
        row_pos = self.xls_write_row(ws, row_pos, row_data, row_style=cell_style_center, set_column_size=True)

        decimal_style = xlwt.easyxf(_xs['italic'], num_format_str=report_xls.decimal_format)
        total_style = xlwt.easyxf(_xs['fill'] + _xs['italic'] + _xs['borders_all'],
                                  num_format_str=report_xls.decimal_format)
        for l in _p.lines(data['form']):
            row_data = self.print_line(l, decimal_style=decimal_style, total_style=total_style)
            row_pos = self.xls_write_row(ws, row_pos, row_data)

        # totals
        cell_format = _xs['bold'] + _xs['fill_blue'] + _xs['borders_all']
        totals_style = xlwt.easyxf(cell_format)
        totals_decimal_style = xlwt.easyxf(cell_format + _xs['italic'], num_format_str=report_xls.decimal_format)
        c_specs = [
            ('ot', 3, 0, 'text', 'TOTAL', None, totals_style),
            ('si_total', 1, 0, 'number', _p.totals()['si_total'], None, totals_decimal_style),
            ('mp_pred_total', 1, 0, 'number', _p.totals()['mp_pred_total'], None, totals_decimal_style),
            ('mp_real_total', 1, 0, 'number', _p.totals()['mp_real_total'], None, totals_decimal_style),
            ('mp_delta_total', 1, 0, 'number', _p.totals()['mp_delta_total'], None, totals_decimal_style),
            ('mod_pred_total', 1, 0, 'number', _p.totals()['mod_pred_total'], None, totals_decimal_style),
            ('mod_real_total', 1, 0, 'number', _p.totals()['mod_real_total'], None, totals_decimal_style),
            ('mod_delta_total', 1, 0, 'number', _p.totals()['mod_delta_total'], None, totals_decimal_style),
            ('se_pred_total', 1, 0, 'number', _p.totals()['se_pred_total'], None, totals_decimal_style),
            ('se_real_total', 1, 0, 'number', _p.totals()['se_real_total'], None, totals_decimal_style),
            ('se_delta_total', 1, 0, 'number', _p.totals()['se_delta_total'], None, totals_decimal_style),
            ('og_pred_total', 1, 0, 'number', _p.totals()['og_pred_total'], None, totals_decimal_style),
            ('og_real_total', 1, 0, 'number', _p.totals()['og_real_total'], None, totals_decimal_style),
            ('og_delta_total', 1, 0, 'number', _p.totals()['og_delta_total'], None, totals_decimal_style),
            ('gi_pred_total', 1, 0, 'number', _p.totals()['gi_pred_total'], None, totals_decimal_style),
            ('gi_real_total', 1, 0, 'number', _p.totals()['gi_real_total'], None, totals_decimal_style),
            ('gi_delta_total', 1, 0, 'number', _p.totals()['gi_delta_total'], None, totals_decimal_style),
            ('pred_total', 1, 0, 'number', _p.totals()['tot_pred'], None, totals_decimal_style),
            ('real_total', 1, 0, 'number', _p.totals()['tot_real'], None, totals_decimal_style),
            ('delta_total', 1, 0, 'number', _p.totals()['tot_delta'], None, totals_decimal_style),
            ('cant_prod_total', 1, 0, 'number', _p.totals()['cant_prod_total'], None, totals_decimal_style),
            ('cant_ven_total', 1, 0, 'number', _p.totals()['cant_vend_total'], None, totals_decimal_style),
            ('prec_vent_total', 1, 0, 'number', _p.totals()['pre_vent_total'], None, totals_decimal_style),
            ('val_ven_total', 1, 0, 'number', _p.totals()['val_vent_total'], None, totals_decimal_style)
        ]
        row_data = self.xls_row_template(c_specs, [x[0] for x in c_specs])
        self.xls_write_row(ws, row_pos, row_data)


orders_sheet_cost_report_xls('report.sale_mrp_account_ingenius_tsj.orders_sheet_cost_xls',
                             'orders.sheet.cost.report.wzd', parser=orders_sheet_cost_report)
