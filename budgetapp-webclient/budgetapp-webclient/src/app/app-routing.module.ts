import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { ConfigComponent } from "./components/config/config.component";
import { DetailsComponent } from "./components/details/details.component";
import { ImportComponent } from "./components/import/import.component";
const routes: Routes = [
  { path: "import", component: ImportComponent },
  { path: "details", component: DetailsComponent },
  { path: "config", component: ConfigComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
