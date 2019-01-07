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


class account_asset_move_line_wzd(osv.osv_memory):
    _name = 'account.asset.move.line.wzd'

    def onchange_asset_id(self, cr, uid, ids, asset_id, context=None):
        res = {'value': {}, 'domain': {}}
        if asset_id:
            asset_pool = self.pool.get('account.asset.asset')
            asset_obj = asset_pool.browse(cr, uid, asset_id, context=context)
            department_id = asset_obj.department_id and asset_obj.department_id.id or False
            responsible_id = asset_obj.responsible_id and asset_obj.responsible_id.id or False
            res.update({'value': {'origin_department_id': department_id,
                                  'old_responsible_id': responsible_id},
                        'domain': {'dest_department_id': [('id', '!=', department_id)],
                                   'new_responsible_id': [('id', '!=', responsible_id)]
                                   }})
        return res

    def onchange_department_id(self, cr, uid, ids, dep_id, context=None):
        res = {'value': {}}
        if dep_id:
            department_pool = self.pool.get('hr.department')
            department_obj = department_pool.browse(cr, uid, dep_id, context=context)
            res['value'].update({
                'new_responsible_id': department_obj.manager_id and department_obj.manager_id.id or False
            })
        return res

    _columns = {
        'origin_department_id': fields.many2one('hr.department', 'Departamento Actual', readonly=True,
                                                help='Área desde la que se traslada el activo.'),
        'old_responsible_id': fields.many2one('hr.employee', 'Responsable Actual', readonly=True,
                                              help='Actual responsable del activo que se traslada.'),
        'dest_department_id': fields.many2one('hr.department', 'Destino',
                                              help='Área hacia la que se traslada el activo.'),
        'new_responsible_id': fields.many2one('hr.employee', 'Nuevo Responsable',
                                              help='Nuevo responsable del activo que se trasladó.'),
        'asset_id': fields.many2one('account.asset.asset', 'Activo',
                                    help='Activo sobre el que se realiza el movimiento.'),
        'move_wzd_id': fields.many2one('account.asset.move.wzd', 'Wzd Ref', required=True,
                                       ondelete='cascade'),
        'old_account_analytic_id': fields.many2one('account.analytic.account', 'Centro de Costo Actual', readonly=True,
                                                   help='Centro de costo al que pertenece el activo.'),
        'account_analytic_id': fields.many2one('account.analytic.account', 'Nuevo Centro de Costo',
                                               help='Nuevo Centro de costo del activo. Dejar vacío para mantener el '
                                                    'centro de costo actual del activo'),
    }


class account_asset_move_wzd(osv.osv_memory):
    _name = 'account.asset.move.wzd'

    def default_get(self, cr, uid, fields, context=None):
        if context is None:
            context = {}
        res = super(account_asset_move_wzd, self).default_get(cr, uid, fields, context=context)
        asset_ids = context.get('active_ids', [])

        if asset_ids:
            lines = []
            asset_pool = self.pool.get('account.asset.asset')
            # draft_ids = asset_pool.search(cr, uid, [('state', 'in', ['draft', 'close'])], context=context)
            # clean_ids = list(set(asset_ids) - set(draft_ids))
            for asset in asset_pool.browse(cr, uid, asset_ids, context=context):
                data = {
                    'asset_id': asset.id,
                    'origin_department_id': asset.department_id.id,
                    'old_responsible_id': asset.responsible_id.id,
                    'old_account_analytic_id': asset.account_analytic_id.id,
                }
                lines.append(data)
            res.update({'line_ids': lines})

        return res

    def move_assets(self, cr, uid, ids, context=None):
        if context is None:
            context = {}
        wzd_obj = self.browse(cr, uid, ids, context=context)
        account_asset_pool = self.pool.get('account.asset.asset')
        for line in wzd_obj.line_ids:
            asset_id = line.asset_id.id
            dp_id = line.dest_department_id.id
            resp_id = line.new_responsible_id.id
            account_analytic_id = line.account_analytic_id and line.account_analytic_id.id or False
            vals = {
                'department_id': dp_id,
                'responsible_id': resp_id,
            }
            if account_analytic_id:
                vals.update({'account_analytic_id': account_analytic_id})
            account_asset_pool.move_asset(cr, uid, asset_id, vals, context=context)

        return True

    _columns = {
        'line_ids': fields.one2many('account.asset.move.line.wzd', 'move_wzd_id', 'Activos a Mover', required=True)
    }
