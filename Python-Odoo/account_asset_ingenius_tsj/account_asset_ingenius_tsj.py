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

import calendar
from datetime import datetime
from openerp import api, exceptions, fields as flds, models, _
from openerp.osv import osv, fields
from openerp.tools import DEFAULT_SERVER_DATE_FORMAT as DATE_FORMAT

from dateutil.relativedelta import relativedelta


class AccountAssetIngenius(osv.osv):
    _inherit = 'account.asset.asset'

    def create_account_move(self, cr, uid, debit_account_id, credit_account_id, amount, date, asset, analytic=False,
                            context=None):
        period_obj = self.pool.get('account.period')
        move_obj = self.pool.get('account.move')
        move_line_obj = self.pool.get('account.move.line')
        currency_obj = self.pool.get('res.currency')
        period_ids = period_obj.find(cr, uid, date, context=context)
        company_currency = asset.company_id.currency_id.id
        current_currency = asset.currency_id.id
        context.update({'date': date})
        _amount = currency_obj.compute(cr, uid, current_currency, company_currency, amount,
                                       context=context)
        sign = (asset.category_id.journal_id.type == 'purchase' and 1) or -1
        asset_name = "/"
        reference = asset.name
        move_vals = {
            'name': asset_name,
            'date': date,
            'ref': reference,
            'period_id': period_ids and period_ids[0] or False,
            'journal_id': asset.category_id.journal_id.id,
        }
        move_id = move_obj.create(cr, uid, move_vals, context=context)
        journal_id = asset.category_id.journal_id.id
        partner_id = asset.partner_id.id
        move_line_obj.create(cr, uid, {
            'name': asset_name,
            'ref': reference,
            'move_id': move_id,
            'account_id': credit_account_id,
            'debit': 0.0,
            'credit': _amount,
            'period_id': period_ids and period_ids[0] or False,
            'journal_id': journal_id,
            'partner_id': partner_id,
            'currency_id': company_currency != current_currency and current_currency or False,
            'amount_currency': company_currency != current_currency and sign * amount or 0.0,
            'date': date,
            'analytic_account_id': context.get('credit', False) and analytic.id or False,
            # 'asset_id': context.get('asset_id', False) and asset.id or False
        })
        move_line_obj.create(cr, uid, {
            'name': asset_name,
            'ref': reference,
            'move_id': move_id,
            'account_id': debit_account_id,
            'debit': _amount,
            'credit': 0.0,
            'period_id': period_ids and period_ids[0] or False,
            'journal_id': journal_id,
            'partner_id': partner_id,
            'currency_id': company_currency != current_currency and current_currency or False,
            'amount_currency': company_currency != current_currency and -sign * amount or 0.0,
            'analytic_account_id': context.get('debit', False) and analytic.id or False,
            'date': date
        })
        move_obj.button_validate(cr, uid, [move_id], context=context)
        return move_id

    def onchange_category_id(self, cr, uid, ids, category_id, context=None):
        res = super(AccountAssetIngenius, self).onchange_category_id(cr, uid, ids, category_id, context=context)
        if category_id:
            asset_categ_obj = self.pool.get('account.asset.category')
            category_obj = asset_categ_obj.browse(cr, uid, category_id, context=context)
            res['value'].update({
                'anual_depreciation_percent': category_obj.anual_depreciation_percent,
                'account_analytic_id': category_obj.account_analytic_id.id,
            })
        return res

    def set_out(self, cr, uid, ids, context=None):
        return self.write(cr, uid, ids, {'state': 'out'}, context=context)

    def move_asset(self, cr, uid, asset_id, vals, context=None):
        asset_obj = self.browse(cr, uid, asset_id, context=context)
        move_vals = {
            'asset_id': asset_id,
            'origin_department_id': asset_obj.department_id and asset_obj.department_id.id or False,
            'dest_department_id': vals.get('department_id', False),
            'old_responsible_id': asset_obj.responsible_id and asset_obj.responsible_id.id or False,
            'new_responsible_id': vals.get('responsible_id', False),
            'old_account_analytic_id': asset_obj.account_analytic_id and asset_obj.account_analytic_id.id or False,
            'account_analytic_id': vals.get('account_analytic_id', False),
            'user_id': uid
        }
        self.pool.get('account.asset.move').create(cr, uid, move_vals, context=context)
        self.write(cr, uid, [asset_id], vals, context=context)

    _columns = {
        'no_inv': fields.char('No. Inventario'),
        'fecha_compra': fields.date('Fecha de Compra'),
        'anual_depreciation_percent': fields.float('Tasa Anual de Depreciación', (3, 2)),
        'method': fields.selection(
            [('linear', 'Linear'), ('degressive', 'Degressive'), ('percent', 'Porciento')],
            'Método de Cálculo', required=True, readonly=True,
            states={'draft': [('readonly', False)], 'open': [('readonly', False)]},
            help="Escoja el método a utilizar para calcular la cantidad de l'neas de amortización.\n" \
                 "  * Porciento: Calculado en base a la tasa de depreciaci'n anual.\n" \
                 "  * Decreciente: Calculado en base a: Valor Residual*Factor Decreciente.\n" \
                 "  * Lineal: Calculado en base a: Valor Bruto/Número de Depreciaciones."),
        'income_move_created': fields.boolean('Asiento de Entrada Generado',
                                              help='Indica que el asiento de Alta de este activo ya ha sido generado'),
        'state': fields.selection(
            [('draft', 'Borrador'), ('open', 'En Ejecución'), ('close', 'Cerrado'), ('out', 'Baja')],
            'Status', required=True, copy=False,
            help="Cuando el activo es creado es puesto en 'Borrador'.\n" \
                 "Cuando el activo se confirma, el estado pasa a 'En Ejecución' y las líneas de depreciacion pueden "
                 "ser asentadas en la contabilidad.\n" \
                 "Usted puede cerrar manualmente un activo cuando la depreciación haya finalizado. Si la última línea "
                 "de depreciación es asentada, el activo automaticamente pasa a 'Cerrado'."),
        'department_id': fields.many2one('hr.department', 'Departamento', help='Área a la que pertenece el activo.',
                                         readonly=True,
                                         states={'draft': [('readonly', False)], 'open': [('readonly', False)]},
                                         select=True),
        'responsible_id': fields.many2one('hr.employee', 'Responsable', help='Responsable del activo.',
                                          states={'draft': [('readonly', False)], 'open': [('readonly', False)]},
                                          select=True),
        # Cuentas de gastos de amortizacion, real y analitica
        'account_expense_depreciation_id': fields.many2one('account.account', 'Cuenta de Gastos de Amortización',
                                                           required=True,
                                                           domain=[('type', '=', 'other')],
                                                           states={'draft': [('readonly', False)],
                                                                   'open': [('readonly', False)]}
                                                           ),
        'analytic_expense_depreciation_id': fields.many2one('account.analytic.account',
                                                            string='Cuenta de Gastos de Amortización Aanalítica',
                                                            required=True, domain=[('type', '=', 'normal')],
                                                            states={'draft': [('readonly', False)],
                                                                    'open': [('readonly', False)]}),
        # Cuenta de inventario analitica
        'analytic_asset_id': fields.many2one('account.analytic.account',
                                             string='Cuenta de Inventario Analítica',
                                             domain=[('type', '=', 'normal')], required=True,
                                             states={'draft': [('readonly', False)],
                                                     'open': [('readonly', False)]}),
        # Cuenta de amortizacion analitica
        'account_analytic_id': fields.many2one('account.analytic.account', 'Cuenta de Amortización Analítica',
                                               help='Cuenta analítica utilizada para la amortización del activo',
                                               readonly=True, domain=[('type', '=', 'normal')], required=True,
                                               states={'draft': [('readonly', False)], 'open': [('readonly', False)]}),
    }

    @api.multi
    def name_get(self):
        result = []
        for inv in self:
            result.append((inv.id, "%s %s" % (inv.no_inv, inv.name or '')))
        return result

    def onchange_method(self, cr, uid, ids, method='percent', context=None):
        res = {'value': {}}
        if method == 'percent':
            res['value'] = {'method_period': 1}
        return res

    def validate(self, cr, uid, ids, context=None):
        to_update_ids = []
        ctx = context.copy()
        ctx.update({'debit': True})
        for asset in self.browse(cr, uid, ids, context=ctx):
            if asset.income_move_created:
                continue
            if asset.value_residual > 0:
                purchase_date = asset.purchase_date
                if fields.datetime.now() >= purchase_date:
                    if not asset.income_move_created:
                        debit_account_id = asset.category_id.account_asset_id.id
                        credit_account_id = asset.category_id.income_account_id.id
                        amount = asset.purchase_value
                        date = purchase_date
                        self.create_account_move(cr, uid, debit_account_id, credit_account_id, amount, date, asset,
                                                 analytic=asset.analytic_asset_id, context=ctx)
                        to_update_ids.append(asset.id)
                else:
                    raise osv.except_osv(_('Error!'),
                                         _(
                                             'La fecha actual es anterior a la fecha de inicio de explotación del activo. '
                                             'Un activo no puede entrar en explotación en una fecha posterior a la de hoy.'))
        del ctx['debit']
        if to_update_ids:
            self.write(cr, uid, to_update_ids, {'income_move_created': True}, context=ctx)
        return super(AccountAssetIngenius, self).validate(cr, uid, ids, context=ctx)

    def write(self, cr, uid, ids, vals, context=None):
        if context is None:
            context = {}
        if not ids:
            return True
        if isinstance(ids, (int, long)):
            ids = [ids]
        if vals.get('anual_depreciation_percent', False):
            adp = float(vals.get('anual_depreciation_percent', False))
            if adp < 0 or adp > 100:
                raise osv.except_osv(_('Error!'), _('La tasa de depreciación anual debe ser un valor entre 0 y 100.'))
        if vals.get('method', False) or vals.get('method_time'):
            self.compute_depreciation_board(cr, uid, ids, context=context)
        return super(AccountAssetIngenius, self).write(cr, uid, ids, vals, context=context)

    def _compute_board_amount(self, cr, uid, asset, i, residual_amount, amount_to_depr, undone_dotation_number,
                              posted_depreciation_line_ids, total_days, depreciation_date, context=None):
        # by default amount = 0
        amount = 0
        prec = 6
        if i == undone_dotation_number:
            amount = residual_amount
        else:
            if asset.method == 'linear':
                amount = amount_to_depr / (undone_dotation_number - len(posted_depreciation_line_ids))
                if asset.prorata:
                    amount = amount_to_depr / asset.method_number
                    days = total_days - float(depreciation_date.strftime('%j'))
                    if i == 1:
                        amount = (amount_to_depr / asset.method_number) / total_days * days
                    elif i == undone_dotation_number:
                        amount = (amount_to_depr / asset.method_number) / total_days * (total_days - days)
            elif asset.method == 'degressive':
                amount = residual_amount * asset.method_progress_factor
                if asset.prorata:
                    days = total_days - float(depreciation_date.strftime('%j'))
                    if i == 1:
                        amount = (residual_amount * asset.method_progress_factor) / total_days * days
                    elif i == undone_dotation_number:
                        amount = (residual_amount * asset.method_progress_factor) / total_days * (total_days - days)
            elif asset.method == 'percent':
                if i == 1:
                    purchase_date = datetime.strptime(asset.purchase_date, DATE_FORMAT)
                    days = 366 if calendar.isleap(depreciation_date.year) else 365
                    daily_percent = round(asset.anual_depreciation_percent / days, prec)
                    daily_value = round(asset.purchase_value * daily_percent / 100, prec)
                    days_to_finish_month = calendar.monthrange(purchase_date.year, purchase_date.month)[
                                               1] - purchase_date.day
                    if calendar.monthrange(purchase_date.year, purchase_date.month)[1] > purchase_date.day > 1:
                        amount = (daily_value * days_to_finish_month)
                    elif purchase_date.day == calendar.monthrange(purchase_date.year, purchase_date.month)[1]:
                        amount = 0
                    elif purchase_date.day == 1:
                        monthly_percent = round(asset.anual_depreciation_percent / 12, prec)
                        amount = round(asset.purchase_value * monthly_percent / 100, prec)
                else:
                    monthly_percent = round(asset.anual_depreciation_percent / 12, prec)
                    amount = round(asset.purchase_value * monthly_percent / 100, prec)
        return amount

    def _compute_board_undone_dotation_nb(self, cr, uid, asset, depreciation_date, total_days, context=None):
        undone_dotation_number = 0 if asset.method == 'percent' else asset.method_number
        prec = 6
        if asset.method_time == 'end':
            end_date = datetime.strptime(asset.method_end, '%Y-%m-%d')
            undone_dotation_number = 0
            while depreciation_date <= end_date:
                depreciation_date = (
                    datetime(depreciation_date.year, depreciation_date.month, depreciation_date.day) + relativedelta(
                        months=+asset.method_period))
                undone_dotation_number += 1
        if asset.method == 'percent':
            amount = asset.purchase_value
            days = 366 if calendar.isleap(depreciation_date.year) else 365
            daily_percent = round(asset.anual_depreciation_percent / days, prec)
            daily_value = round(amount * daily_percent / 100, prec)
            monthly_percent = round(asset.anual_depreciation_percent / 12, prec)
            monthly_value = round(amount * monthly_percent / 100, prec)
            undone_dotation_number = 0
            days_to_finish_month = calendar.monthrange(depreciation_date.year, depreciation_date.month)[
                                       1] - depreciation_date.day
            if depreciation_date.day > 1:
                undone_dotation_number += 1
                if depreciation_date.day < calendar.monthrange(depreciation_date.year, depreciation_date.month)[1]:
                    amount -= (daily_value * days_to_finish_month)
            while amount > asset.salvage_value:
                undone_dotation_number += 1
                amount -= monthly_value

        if asset.prorata and asset.method == 'linear':
            undone_dotation_number += 1
        return undone_dotation_number

    def compute_depreciation_board(self, cr, uid, ids, context=None):
        depreciation_lin_obj = self.pool.get('account.asset.depreciation.line')
        currency_obj = self.pool.get('res.currency')
        for asset in self.browse(cr, uid, ids, context=context):
            if asset.value_residual == 0.0:
                continue
            posted_depreciation_line_ids = depreciation_lin_obj.search(cr, uid, [('asset_id', '=', asset.id),
                                                                                 ('move_check', '=', True)],
                                                                       order='depreciation_date desc')
            old_depreciation_line_ids = depreciation_lin_obj.search(cr, uid, [('asset_id', '=', asset.id),
                                                                              ('move_id', '=', False)])
            if old_depreciation_line_ids:
                depreciation_lin_obj.unlink(cr, uid, old_depreciation_line_ids, context=context)

            amount_to_depr = residual_amount = asset.value_residual
            if asset.prorata or asset.method == 'percent':
                depreciation_date = datetime.strptime(
                    self._get_last_depreciation_date(cr, uid, [asset.id], context)[asset.id], '%Y-%m-%d')
            else:
                # depreciation_date = 1st January of purchase year
                purchase_date = datetime.strptime(asset.purchase_date, '%Y-%m-%d')
                # if we already have some previous validated entries, starting date isn't 1st January but last entry + method period
                if (len(posted_depreciation_line_ids) > 0):
                    last_depreciation_date = datetime.strptime(
                        depreciation_lin_obj.browse(cr, uid, posted_depreciation_line_ids[0],
                                                    context=context).depreciation_date, '%Y-%m-%d')
                    depreciation_date = (last_depreciation_date + relativedelta(months=+asset.method_period))
                else:
                    depreciation_date = datetime(purchase_date.year, 1, 1)

            day = depreciation_date.day
            month = depreciation_date.month
            year = depreciation_date.year
            total_days = (year % 4) and 365 or 366

            undone_dotation_number = self._compute_board_undone_dotation_nb(cr, uid, asset, depreciation_date,
                                                                            total_days, context=context)
            if asset.method == 'percent':
                month_range = calendar.monthrange(depreciation_date.year, depreciation_date.month)
                depreciation_date = datetime(depreciation_date.year, depreciation_date.month, month_range[1])
                day = depreciation_date.day

            for x in range(len(posted_depreciation_line_ids), undone_dotation_number):
                i = x + 1
                amount = self._compute_board_amount(cr, uid, asset, i, residual_amount, amount_to_depr,
                                                    undone_dotation_number, posted_depreciation_line_ids, total_days,
                                                    depreciation_date, context=context)
                residual_amount -= amount
                vals = {
                    'amount': amount,
                    'asset_id': asset.id,
                    'sequence': i,
                    'name': str(asset.id) + '/' + str(i),
                    'remaining_value': residual_amount,
                    'depreciated_value': (asset.purchase_value - asset.salvage_value) - (residual_amount + amount),
                    'depreciation_date': depreciation_date.strftime('%Y-%m-%d'),
                }
                depreciation_lin_obj.create(cr, uid, vals, context=context)
                # Considering Depr. Period as months
                depreciation_date = (datetime(year, month, day) + relativedelta(months=+asset.method_period))
                year = depreciation_date.year
                month = depreciation_date.month
                if asset.method == 'percent':
                    day = calendar.monthrange(year, month)[1]
                    depreciation_date = datetime(year, month, day)
                else:
                    day = depreciation_date.day
        return True

    _defaults = {
        'method': 'percent',
        'purchase_date': False,
        'method_period': 1,
        'anual_depreciation_percent': 100,
    }


class AccountAssetDepreciationLine(models.Model):
    _inherit = 'account.asset.depreciation.line'

    @api.multi
    def create_move(self):
        asset_obj = self.env['account.asset.asset']
        period_obj = self.env['account.period']
        move_obj = self.env['account.move']
        created_move_ids = []
        asset_ids = []
        for line in self:
            depreciation_date = self.env.context.get(
                'depreciation_date') or line.depreciation_date or flds.Date.context_today(self)
            period = period_obj.find(depreciation_date)
            comp_currency = line.asset_id.company_id.currency_id
            asset_currency = line.asset_id.currency_id
            amount = asset_currency.with_context(date=depreciation_date).compute(line.amount, comp_currency)
            sign = (line.asset_id.category_id.journal_id.type == 'purchase' and 1) or -1
            asset_name = "/"
            reference = line.asset_id.name
            move_vals = {
                'name': asset_name,
                'date': depreciation_date,
                'ref': reference,
                'period_id': period and period.id or False,
                'journal_id': line.asset_id.category_id.journal_id.id,
            }
            journal_id = line.asset_id.category_id.journal_id.id
            partner_id = line.asset_id.partner_id.id
            currency_id = comp_currency.id != asset_currency.id and asset_currency.id or False
            line_vals1 = {
                'name': asset_name,
                'ref': reference,
                'account_id': line.asset_id.category_id.account_depreciation_id.id,
                'debit': 0.0,
                'credit': amount,
                'period_id': period and period.id or False,
                'journal_id': journal_id,
                'partner_id': partner_id,
                'amount_currency': comp_currency.id != asset_currency.id and - sign * line.amount or 0.0,
                'analytic_account_id': line.asset_id.account_analytic_id.id,
                'date': depreciation_date,
            }
            line_vals2 = {
                'name': asset_name,
                'ref': reference,
                'account_id': line.asset_id.account_expense_depreciation_id.id,
                'credit': 0.0,
                'debit': amount,
                'period_id': period and period.id or False,
                'journal_id': journal_id,
                'partner_id': partner_id,
                'amount_currency': comp_currency.id != asset_currency.id and sign * line.amount or 0.0,
                'analytic_account_id': line.asset_id.analytic_expense_depreciation_id.id,
                'date': depreciation_date,
                'asset_id': line.asset_id.id
            }
            if currency_id:
                line_vals1.update({'currency_id': currency_id})
                line_vals2.update({'currency_id': currency_id})

            move_vals.update({'line_id': [(0, 0, line_vals1), (0, 0, line_vals2)]})
            move = move_obj.create(move_vals)
            line.move_id = move.id
            created_move_ids.append(move.id)
            asset_ids.append(line.asset_id.id)

        # we re-evaluate the assets to determine whether we can close them
        for asset in asset_obj.browse(list(set(asset_ids))):
            if asset.currency_id.is_zero(asset.value_residual):
                asset.write({'state': 'close'})

        return created_move_ids


class AccountAssetCategory(models.Model):
    _inherit = 'account.asset.category'

    @api.onchange('method')
    def onchange_method(self):
        if self.method == 'percent':
            self.method_period = 1

    income_account_id = flds.Many2one('account.account', 'Cuenta de Alta', required=True,
                                      domain=[('type', '=', 'payable')],
                                      help='Cuenta acreedora por cuando se le da alta a un activo de esta categoria.')
    anual_depreciation_percent = flds.Float('Tasa Anual de Depreciación', (3, 2), default=100)
    method = flds.Selection([('linear', 'Linear'), ('degressive', 'Degressive'), ('percent', 'Porciento')],
                            'Método de Cálculo', required=True, default='percent',
                            help="Escoja el método a utilizar para calcular la cantidad de l'neas de amortización.\n" \
                                 "  * Porciento: Calculado en base a la tasa de depreciación anual.\n" \
                                 "  * Decreciente: Calculado en base a: Valor Residual*Factor Decreciente.\n" \
                                 "  * Lineal: Calculado en base a: Valor Bruto/Número de Depreciaciones.")
    open_account_id = flds.Many2one('account.account', 'Cuenta de Apertura', required=True,
                                    domain=[('type', '=', 'other')], help='Cuenta para el asiento de apertura.')
    method_period = flds.Integer(default=1)

    @api.multi
    def unlink(self):
        account_asset_env = self.env['account.asset.asset']
        assets_count = account_asset_env.search_count([('category_id', 'in', self.ids)])
        if assets_count:
            raise exceptions.Warning(_('Está tratando de eliminar una categoría que tiene activos asociados. '
                                       'Debe cambiar la categoría de los activos implicados.'))
        return super(AccountAssetCategory, self).unlink()


class AccountAssetMove(models.Model):
    _name = 'account.asset.move'
    _rec_name = 'asset_id'

    origin_department_id = flds.Many2one('hr.department', 'Origen', readonly=True,
                                         help='Área desde la que se traslada el activo.')
    old_responsible_id = flds.Many2one('hr.employee', 'Antiguo Responsable', readonly=True,
                                       help='Antiguo responsable del activo que se trasladó.')
    dest_department_id = flds.Many2one('hr.department', 'Destino', readonly=True,
                                       help='Área hacia la que se traslada el activo.')
    new_responsible_id = flds.Many2one('hr.employee', 'Nuevo Responsable', readonly=True,
                                       help='Nuevo responsable del activo que se trasladó.')
    asset_id = flds.Many2one('account.asset.asset', 'Activo', readonly=True,
                             help='Activo sobre el que se realiza el movimiento.')
    asset_move_date = flds.Datetime('Fecha de Traslado', readonly=True, default=flds.Datetime.now,
                                    help='Fecha en la que se realiza el traslado del activo')
    user_id = flds.Many2one('res.users', 'Usuario', readonly=True, default=lambda self: self.env.user,
                            help='Usuario que resgistra el traslado del activo')
    old_account_analytic_id = flds.Many2one('account.analytic.account', 'Antiguo Centro de Costo', readonly=True,
                                            help='Antiguo centro de costo del activo que se trasladó.')
    account_analytic_id = flds.Many2one('account.analytic.account', 'Nuevo Centro de Costo', readonly=True,
                                        help='Nuevo centro de costo del activo que se trasladó.')
