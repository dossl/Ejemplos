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

from openerp.osv import osv, fields
import openerp.addons.decimal_precision as dp


class account_asset_out_line_wzd(osv.osv_memory):
    _name = 'account.asset.out.line.wzd'

    _columns = {
        'asset_id': fields.many2one('account.asset.asset', 'Activo Fijo'),
        'asset_category_id': fields.related('asset_id', 'category_id', type='many2one',
                                            relation='account.asset.category', string='Categoría', readonly=True),
        'purchase_date': fields.date('Inicio de Explotación'),
        'purchase_value': fields.float('Valor Bruto', digits_compute=dp.get_precision('Account')),
        'value_residual': fields.float('Valor Residual', digits_compute=dp.get_precision('Account')),
        'out_id': fields.many2one('account.asset.out.wzd', 'Wzd Ref', required=True,
                                  ondelete='cascade'),
    }


class account_asset_out_wzd(osv.osv_memory):
    _name = 'account.asset.out.wzd'

    def default_get(self, cr, uid, fields, context=None):
        if context is None:
            context = {}
        res = super(account_asset_out_wzd, self).default_get(cr, uid, fields, context=context)
        asset_ids = context.get('active_ids', [])

        if asset_ids:
            lines = []
            asset_pool = self.pool.get('account.asset.asset')
            draft_ids = asset_pool.search(cr, uid, [('state', 'in', ['draft', 'close'])], context=context)
            clean_ids = list(set(asset_ids) - set(draft_ids))
            for asset in asset_pool.browse(cr, uid, clean_ids, context=context):
                data = {
                    'asset_id': asset.id,
                    'asset_category_id': asset.category_id.id,
                    'purchase_date': asset.purchase_date,
                    'purchase_value': asset.purchase_value,
                    'value_residual': asset.value_residual,
                }
                lines.append(data)
            res.update({'line_ids': lines})

        return res

    def set_out(self, cr, uid, ids, context=None):
        if context is None:
            context = {}
        wzd_obj = self.browse(cr, uid, ids, context=context)
        asset_ids = [line.asset_id.id for line in wzd_obj.line_ids]
        if asset_ids:
            account_asset_pool = self.pool.get('account.asset.asset')
            account_asset_pool.set_out(cr, uid, asset_ids, context=context)
        return True

    _columns = {
        'line_ids': fields.one2many('account.asset.out.line.wzd', 'out_id', 'Activos a dar Baja')
    }
