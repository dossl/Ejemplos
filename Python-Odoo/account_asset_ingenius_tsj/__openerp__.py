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


{
    'name': 'Activos Fijos',
    'version': '1.1',
    'category': 'Accounting',
    'description': u"""
Este Módulo complementa la gestión de Activos Fijos.
=====================================================

Nuevas Funcionalidades:
    - Nuevo método de cálculo de la depreciación del activo. (Porcentaje Anual).
    - Realizar Altas y Bajas de Activos Fijos.
    - Movimiento de Activos Fijos entre los diferentes departamentos.
       """,
    'author': 'Ingenius',
    'website': 'https://www.ingeniuscuba.com',
    'depends': ['account_asset', 'account_accountant', 'account_analytic_analysis'],
    'data': [
        'wizard/account_asset_batch_depreciation_wzd_view.xml',
        'wizard/account_asset_execution_wzd_view.xml',
        # 'wizard/account_asset_out_wzd_view.xml',
        'wizard/account_asset_move_wzd_view.xml',
        'wizard/account_asset_open_moves_wzd_view.xml',
        'views/account_asset_asset_ingenius_tsj_view.xml'
    ],
    'installable': True,
    'auto_install': False,
}

# vim:expandtab:smartindent:tabstop=4:softtabstop=4:shiftwidth=4:
