import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from "./home/home.component";
import { DashboardComponent } from "./dashboard/dashboard.component";
import { MessagesComponent } from "./messages/messages/messages.component";
import { DeadLettersComponent } from "./deadletters/deadletters/deadletters.component";


const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'healthcheck', component: DashboardComponent },
  { path: 'messages', component: MessagesComponent },
  { path: 'deadletters', component: DeadLettersComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}

