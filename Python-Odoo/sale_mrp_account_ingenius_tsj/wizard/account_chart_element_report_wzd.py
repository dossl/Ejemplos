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

from openerp import api, exceptions, fields, models, _

TYPES = [('subdetail', 'Detalle por Elemento'), ('detail', 'Detalle por Cuentas'), ('summary', 'Resumen')]


class account_chart_element_report_wzd(models.TransientModel):
    _name = 'account.chart.element.report.wzd'

    type = fields.Selection(TYPES, string='Tipo de Reporte')
    date_from = fields.Date(string='Desde',
                            help='Rango inferior de búsqueda de gastos. Dejar vacío para no establecer límite')
    date_to = fields.Date(string='Desde',
                          help='Rango superior de búsqueda de gastos. Dejar vacío para no establecer límite')

    @api.multi
    def print_report(self):
        self.ensure_one()
        data = {'ids': self.env.context.get('active_ids', [])}
        res = self.read(['type', 'date_from', 'date_to'])
        res = res and res[0] or {}
        data['form'] = res
        domain = []
        config_ids = self.env['sale.offer.config'].search([], limit=1)
        if not config_ids:
            msg = u'Debe establecer los parámetros de configuración de la oferta de venta en el menú ' \
                  u'Fabricación/Configuración/Configuración de Oferta'
            raise exceptions.Warning(msg)
        data['form']['in_process_id'] = config_ids[0].analytic_account_id and config_ids[
            0].analytic_account_id.id or False
        if res['date_from']:
            domain.append(('date', '>=', res['date_from']))
        if res['date_to']:
            domain.append(('date', '<=', res['date_to']))
        line_ids = self.env['account.analytic.line'].search(domain)
        if not line_ids:
            raise exceptions.Warning(u'No hay gastos registrados en el período seleccionado.')
        data['form']['ids'] = line_ids.ids
        if res.get('id', False):
            data['ids'] = [res['id']]

        return self.env['report'].get_action(self, 'sale_mrp_account_ingenius_tsj.report_account_chart_element_tsj',
                                             data=data)
