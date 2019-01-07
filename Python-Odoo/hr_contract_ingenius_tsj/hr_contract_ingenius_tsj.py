# -*- coding:utf-8 -*-
##############################################################################
#
#    OpenERP, Open Source Management Solution
#    Copyright (C) 2011 OpenERP SA (<http://openerp.com>). All Rights Reserved
#
#    This program is free software: you can redistribute it and/or modify
#    it under the terms of the GNU Affero General Public License as published by
#    the Free Software Foundation, either version 3 of the License, or
#    (at your option) any later version.
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
from openerp import fields
from openerp.osv import fields, osv

CONTRACT_STATES_OPTIONS = [('draft', 'Borrador'),
                           ('approved', 'Aprobado'),
                           ('closed', 'Terminado')]

CONTRACT_TYPE = [('determinate', 'Determinado'),
                 ('indeterminate', 'Indeterminado')]

AMOUNT_TYPE = [('percent', 'Porciento'), ('coef', 'Coeficiente'), ('plus', 'Plus'), ('fix', 'Fijo')]


class hr_employee(osv.osv):
    _inherit = 'hr.employee'

    def get_contract(self, cr, uid, employee, date_from, date_to, context=None):
        """
        @param employee: browse record of employee
        @param date_from: date field
        @param date_to: date field
        @return: returns the ids of all the contracts for the given employee that need to be considered for the given dates
        """
        contract_obj = self.pool.get('hr.contract')
        clause = []
        # a contract is valid if it ends between the given dates
        clause_1 = ['&', ('date_end', '<=', date_to), ('date_end', '>=', date_from)]
        # OR if it starts between the given dates
        clause_2 = ['&', ('date_start', '<=', date_to), ('date_start', '>=', date_from)]
        # OR if it starts before the date_from and finish after the date_end (or never finish)
        clause_3 = ['&', ('date_start', '<=', date_from), '|', ('date_end', '=', False), ('date_end', '>=', date_to)]
        clause_final = [('employee_id', '=', employee.id), '|', '|'] + clause_1 + clause_2 + clause_3
        contract_ids = contract_obj.search(cr, uid, clause_final, context=context)
        return contract_ids


class hr_contract(osv.osv):
    _inherit = 'hr.contract'
    _rec = 'name'

    def set_approved(self, cr, uid, ids, context=None):
        return self.write(cr, uid, ids, {'state': 'approved'}, context=context)

    def set_closed(self, cr, uid, ids, context=None):
        return self.write(cr, uid, ids, {'state': 'closed'}, context=context)

    def set_draft(self, cr, uid, ids, context=None):
        return self.write(cr, uid, ids, {'state': 'draft'}, context=context)

    def _in_trial(self, cr, uid, ids, field_name, arg, context=None):
        res = {}
        if context is None:
            context = {}
        today = fields.date.today()
        for c in self.browse(cr, uid, ids, context=context):
            flag = False
            if c.trial_date_start and c.trial_date_end and (c.trial_date_start <= today <= c.trial_date_end):
                flag = True
            res[c.id] = flag
        return res

    def _check_for_free(self, cr, uid, ids, context=None):
        dom = [('contract_id', 'in', ids), ('for_free', '=', True)]
        count = self.pool.get('hr.contract.tarifa').search_count(cr, uid, dom)
        if count > 1:
            return False
        return True

    _columns = {
        'state': fields.selection(CONTRACT_STATES_OPTIONS, string='State', readonly=True),
        'contract_type': fields.selection(CONTRACT_TYPE, string='Tipo de Contrato', readonly=True,
                                          states={'draft': [('readonly', False)]}),
        'department_id': fields.related('employee_id, department_id', type='many2one', relation='hr.department',
                                        readonly=True, states={'draft': [('readonly', False)]}),
        'contract_tarifa_ids': fields.one2many('hr.contract.tarifa', 'contract_id',
                                               'Condiciones de las Tarifas Horarias', readonly=True,
                                               states={'draft': [('readonly', False)]}),
        'destajo': fields.boolean('Pago a Destajo', help='Indica si este contrato incluirá el pago a destajo.',
                                  readonly=True, states={'draft': [('readonly', False)]}),
        'in_trial': fields.function(_in_trial, string='En Período de Prueba', type='boolean',
                                    help="Determina si un contrato se encuentra en período de prueba."),
        'user_id': fields.related('employee_id', 'user_id', type='many2one', relation='res.users', store=True),
        'wage': fields.float('Wage', digits=(16,8), required=True, help="Basic Salary of the employee")
    }

    _constraints = [
        (_check_for_free, 'La tarifa de feriados debe ser única por contrato.', ['contract_tarifa_ids']),
    ]

    _defaults = {
        'state': 'draft'
    }


class hr_contract_hour_type(osv.osv):
    _name = 'hr.hour.type'
    _rec = 'name'
    _order = 'default_value asc'

    _columns = {
        'name': fields.char('Nombre', required=True),
        'code': fields.char('Código', required=True),
        'default_type': fields.selection(AMOUNT_TYPE, string='Operación por Defecto', required=True,
                                         help='Operación por defecto para este tipo de hora.'),
        'default_value': fields.float('Valor por Defecto', help='Valor por defecto de este tipo de hora.')
    }


class hr_contract_tarifa(osv.osv):
    _name = 'hr.contract.tarifa'
    _rec = 'name'
    _order = 'amount_qty asc'

    def _get_total(self, wage, qty, amount_type):
        total = 0
        if amount_type == 'percent':
            total = wage * qty / 100
        elif amount_type == 'coef':
            total = wage * qty
        elif amount_type == 'plus':
            total = wage + qty
        elif amount_type == 'fix':
            total = qty
        return total

    def onchange_hour_type_id(self, cr, uid, ids, hour_id, wage, context=None):
        res = {'value': {}}
        if hour_id:
            hour_type_pool = self.pool.get('hr.hour.type')
            hour_obj = hour_type_pool.browse(cr, uid, hour_id, context)
            _amount_qty = hour_obj.default_value or 0
            _amount_type = hour_obj.default_type or False

            res['value'].update({
                'amount_qty': _amount_qty,
                'amount_type': _amount_type,
            })

        return res

    def onchange_vals(self, cr, uid, ids, amount_type, amount_qty, wage, context=None):
        res = {'value': {}}
        if amount_type and amount_qty:
            total = self._get_total(wage, amount_qty, amount_type)
            res['value'].update({
                'total': total,
            })

        return res

    def onchange_amount_qty(self, cr, uid, ids, hour_id, wage, context=None):
        res = {'value': {}}
        if hour_id:
            hour_type_pool = self.pool.get('hr.hour.type')
            hour_obj = hour_type_pool.browse(cr, uid, hour_id, context)
            _amount_qty = hour_obj.default_value or 0
            _amount_type = hour_obj.default_type or 0
            # total = self._get_total(wage, _amount_qty, _amount_type)

            res['value'].update({
                'amount_qty': _amount_qty,
                'amount_type': _amount_type,
                # 'total': total,
            })

        return res

    def _get_tarifa_from_contract(self, cr, uid, ids, context=None):
        contract_pool = self.pool.get('hr.contract')
        contract_objs = contract_pool.browse(cr, uid, ids, context=context)
        tarifa_ids = []
        for contract in contract_objs:
            tarifa_ids.extend([line.id for line in contract.contract_tarifa_ids])
        return tarifa_ids

    def _total(self, cr, uid, ids, field_name, arg, context=None):
        # TODO: Revisar lo de los impuestos internos de los productos
        res = {}
        if context is None:
            context = {}
        for line in self.browse(cr, uid, ids, context=context):
            total = self._get_total(line.contract_id.wage, line.amount_qty, line.amount_type)
            res[line.id] = total
        return res

    _columns = {
        'hour_type_id': fields.many2one('hr.hour.type', 'Tipo de Hora', required=True),
        'amount_type': fields.selection(AMOUNT_TYPE, string='Operación', required=True),
        'amount_qty': fields.float('Cantidad', required=True),
        'for_free': fields.boolean('Tarifa de Feriado'),
        'total': fields.function(_total, string='Total', digits=(16,8),
                                 store={
                                     'hr.contract': (
                                         _get_tarifa_from_contract, ['wage'], 10),
                                     'hr.contract.tarifa': (
                                         lambda self, cr, uid, ids, c={}: ids, ['amount_type', 'amount_qty'], 10),
                                 }, help="Total de la tarifa horaria."),
        'contract_id': fields.many2one('hr.contract', 'Contrato')
    }

    _sql_constraints = [
        ('name', 'unique (contract_id, hour_type_id)', 'El tipo de hora debe ser única por contrato!'),
    ]

# vim:expandtab:smartindent:tabstop=4:softtabstop=4:shiftwidth=4:
