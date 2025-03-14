import { NavItem } from './nav-item/nav-item';

export const navItems: NavItem[] = [
  {
    navCap: 'Dashboard',
  },
  {
    displayName: 'Dashboard',
    iconName: 'material-symbols-light:dashboard-outline',
    route: 'pages/dashboard',
  },
  {
    navCap: 'Categorias',
    divider: true
  },
  {
    displayName: 'Manutenção',
    iconName: 'material-symbols-light:engineering-outline',
    route: 'pages/category/list',
  },
  {
    navCap: 'Planejamento de gastos',
    divider: true
  },
  {
    displayName: 'Por categoria',
    iconName: 'fluent:money-hand-16-regular',
    route: 'pages/budget/by-category',
  },
  {
    displayName: 'Geral',
    iconName: 'solar:money-bag-linear',
    route: 'pages/budget/general',
  },
  {
    navCap: 'Transações',
    divider: true
  },
  {
    displayName: 'Lançamentos',
    iconName: 'tabler:transaction-dollar',
    route: 'pages/transaction/list',
  },
  {
    navCap: 'Relatórios:',
    divider: true
  },
  // {
  //   displayName: 'Gastos por período',
  //   iconName: 'material-symbols-light:date-range-outline',
  //   route: '/dashboard-',
  // },
  {
    displayName: 'Gastos por Categoria',
    iconName: 'material-symbols-light:date-range-outline',
    route: 'pages/report/category',
  }
];
