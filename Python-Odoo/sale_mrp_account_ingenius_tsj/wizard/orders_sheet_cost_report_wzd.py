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


class orders_sheet_cost_report_wzd(models.TransientModel):
    _name = 'orders.sheet.cost.report.wzd'

    @api.multi
    def xls_export(self):
        self.ensure_one()
        data = self.read()[0]
        data.update({'ids': self.env.context.get('active_ids', [])})
        datas = {
            'ids': self.env.context.get('active_ids', []),
            'form': data
        }
        return {
            'type': 'ir.actions.report.xml',
            'report_name': 'sale_mrp_account_ingenius_tsj.orders_sheet_cost_xls',
            'datas': datas,
        }
