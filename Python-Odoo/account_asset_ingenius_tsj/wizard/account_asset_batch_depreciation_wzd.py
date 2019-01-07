# -*- coding: utf-8 -*-
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
from openerp import api, fields, models


class AssetDepreciationWzd(models.TransientModel):
    _name = 'asset.depreciation.batch.wzd'

    @api.multi
    def batch_depreciation(self):
        self.ensure_one()
        asset_pool = self.env['account.asset.asset']
        assets = asset_pool.search([('state', '=', 'open')])
        if assets:
            depreciation_line_pool = self.env['account.asset.depreciation.line']
            dom = [('asset_id', 'in', assets.ids), ('move_check', '=', False), ('depreciation_date', '<=', self.date)]
            line_ids = depreciation_line_pool.search(dom, order='depreciation_date asc')
            if line_ids:
                line_ids.create_move()
        return True

    date = fields.Date(string='Fecha', default=fields.Date.context_today)
