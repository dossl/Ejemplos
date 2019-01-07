# -*- coding: utf-8 -*-

{
    'name': 'Integración Contabilidad-Fabricación',
    'version': '1.0',
    'category': 'Sales Management',
    'sequence': 14,
    'description': """
        Módulo de integración entre fabricación y contabilidad.
    """,
    'author': 'Ingenius',
    'website': 'http://www.ingeniuscuba.com',
    'depends': ['account_ingenius_tsj', 'stock_ingenius_tsj'],
    'data': [
        'wizard/account_chart_element_report_wzd.xml',
        'wizard/orders_sheet_cost_report_wzd.xml',
        'views/report_account_chart_element.xml',
    ],
    'demo': [],
    'test': [],
    'installable': True,
    'auto_install': True,
    'application': False,
}
# vim:expandtab:smartindent:tabstop=4:softtabstop=4:shiftwidth=4:
