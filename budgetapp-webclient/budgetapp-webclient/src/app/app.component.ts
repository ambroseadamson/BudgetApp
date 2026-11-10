import { Component } from '@angular/core';
import { IsDataLoadedService } from './services/is-data-loaded.service';

@Component({
  standalone: false,
  selector: 'budgetapp-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Budget Manager';
  dark = false;
  isDataLoaded = false;

  constructor(private isDataLoadedService: IsDataLoadedService) {
    this.isDataLoadedService.currentData.subscribe(
      data => (this.isDataLoaded = data)
    );
  }
}
