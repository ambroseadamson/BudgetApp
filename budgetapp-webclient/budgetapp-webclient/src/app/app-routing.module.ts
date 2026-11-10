import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';



const routes: Routes = [
  { path: 'import', loadComponent: () => import('./components/import/import.component').then(m => m.ImportComponent) },
  { path: 'details', loadComponent: () => import('./components/details/details.component').then(m => m.DetailsComponent) },
  { path: 'config', loadComponent: () => import('./components/config/config.component').then(m => m.ConfigComponent) }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
