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


class asset_open_moves_wzd(osv.osv_memory):
    _name = 'asset.open.moves.wzd'

    def open_moves(self, cr, uid, ids, context=None):
        if context is None:
            context = {}
        wzd_obj = self.browse(cr, uid, ids, context=context)
        asset_pool = self.pool.get('account.asset.asset')
        active_ids = context.get('active_ids', [])
        opened_ids = []

        def _generate_open_moves(a):
            # Primer Asiento
            debit_account_id = a.category_id.account_asset_id.id
            credit_account_id = a.category_id.open_account_id.id
            amount = a.purchase_value
            analytic_account = a.analytic_asset_id
            context.update({'debit': True})
            asset_pool.create_account_move(cr, uid, debit_account_id, credit_account_id, amount, wzd_obj.date, a,
                                           analytic=analytic_account, context=context)
            del context['debit']

            # Segundo Asiento
            debit_account_id = a.category_id.open_account_id.id
            credit_account_id = a.category_id.account_depreciation_id.id
            amount = a.salvage_value
            analytic_account = a.account_analytic_id
            context.update({'credit': True})
            asset_pool.create_account_move(cr, uid, debit_account_id, credit_account_id, amount, wzd_obj.date, a,
                                           analytic=analytic_account, context=context)
            del context['credit']

        asset_ids = asset_pool.search(cr, uid, [('id', 'in', active_ids), ('income_move_created', '=', False)],
                                      context=context)
        if asset_ids:
            for asset in asset_pool.browse(cr, uid, asset_ids, context=context):
                open_id = asset.category_id.open_account_id
                cat = asset.category_id
                if not cat:
                    msg = u'El activo fijo %s no tiene la categoría definida' % asset.no_inv
                    raise osv.except_osv('Error!', msg)
                if not open_id:
                    msg = u'La categoría de activos fijos %s no tiene definida la cuenta de apertura' % cat.name
                    raise osv.except_osv('Error!', msg)
                _generate_open_moves(asset)
                opened_ids.append(asset.id)
            if opened_ids:
                asset_pool.write(cr, uid, opened_ids, {'income_move_created': True}, context=context)
        else:
            msg = u'Todos los activos seleccionados tienen el asiento de apertura generado.'
            raise osv.except_osv('Error!', msg)

        return True

    _columns = {
        'date': fields.date('Fecha', help='Fecha de los asientos contables relacionados con la apertura.'),
    }
