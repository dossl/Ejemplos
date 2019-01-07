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

import openerp.addons.decimal_precision as dp
from openerp.osv import osv, fields


class asset_depreciation_line_wzd(osv.osv_memory):
    _name = 'asset.depreciation.line.wzd'

    _columns = {
        'remaining_value': fields.float('Valor Residual', digits_compute=dp.get_precision('Account')),
        'depreciated_value': fields.float('Depreciación Acumulada'),
        'depreciation_date': fields.date('Fecha de Depreciación'),
        'amount': fields.float('Depreciación', digits_compute=dp.get_precision('Account')),
        'line_reference': fields.integer('Linea de Depreciación'),
        'dep_wzd_id': fields.many2one('asset.depreciation.wzd', 'Wzd Ref', required=True, ondelete='cascade'),
        'asset_id': fields.many2one('account.asset.asset', 'Activo Fijo'),
    }


class asset_depreciation_wzd(osv.osv_memory):
    _name = 'asset.depreciation.wzd'

    def default_get(self, cr, uid, fields, context=None):
        if context is None:
            context = {}
        res = super(asset_depreciation_wzd, self).default_get(cr, uid, fields, context=context)
        asset_ids = context.get('active_ids', [])

        if asset_ids:
            asset_pool = self.pool.get('account.asset.asset')
            draft_ids = asset_pool.search(cr, uid, [('state', 'in', ['draft', 'close'])], context=context)
            clean_ids = list(set(asset_ids) - set(draft_ids))
            depreciation_line_pool = self.pool.get('account.asset.depreciation.line')
            depreciation_line_ids = []
            for asset in asset_pool.browse(cr, uid, clean_ids, context=context):
                _dom = [('asset_id', '=', asset.id), ('move_check', '=', False)]
                next_depreciation_line_ids = depreciation_line_pool.search(cr, uid, _dom, order='depreciation_date asc',
                                                                           limit=1, context=context)
                if next_depreciation_line_ids:
                    depreciation_line_ids.append(next_depreciation_line_ids[0])

            if depreciation_line_ids:
                lines = []
                for line in depreciation_line_pool.browse(cr, uid, depreciation_line_ids, context=context):
                    data = {
                        'line_reference': line.id,
                        'depreciated_value': line.depreciated_value,
                        'depreciation_date': line.depreciation_date,
                        'amount': line.amount,
                        'remaining_value': line.remaining_value,
                        'asset_id': line.asset_id.id
                    }
                    lines.append(data)
                res.update({'line_ids': lines})

        return res

    def batch_depreciation(self, cr, uid, ids, context=None):
        if context is None:
            context = {}
        wzd_obj = self.browse(cr, uid, ids, context=context)
        depreciation_line_ids = [line.line_reference for line in wzd_obj.line_ids]
        if depreciation_line_ids:
            depreciation_line_pool = self.pool.get('account.asset.depreciation.line')
            depreciation_line_pool.create_move(cr, uid, depreciation_line_ids, context=context)
        return True

    _columns = {
        'period_start_date': fields.date('Fecha de Inicio'),
        'period_end_date': fields.date('Fecha de Fin'),
        'line_ids': fields.one2many('asset.depreciation.line.wzd', 'dep_wzd_id', 'Líneas de Depreciación')
    }
