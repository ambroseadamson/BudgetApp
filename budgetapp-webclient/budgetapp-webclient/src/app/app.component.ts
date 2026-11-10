import { Component, inject } from '@angular/core';
import { IsDataLoadedService } from './services/is-data-loaded.service';
import { MatToolbar, MatToolbarRow } from '@angular/material/toolbar';
import { MatButton } from '@angular/material/button';
import { RouterLinkActive, RouterLink, RouterOutlet } from '@angular/router';

@Component({
    selector: 'budgetapp-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css'],
    imports: [MatToolbar, MatToolbarRow, MatButton, RouterLinkActive, RouterLink, RouterOutlet]
})
export class AppComponent {
  private isDataLoadedService = inject(IsDataLoadedService);

  title = 'Budget Manager';
  dark = false;
  isDataLoaded = false;

  /** Inserted by Angular inject() migration for backwards compatibility */
  constructor(...args: unknown[]);

  constructor() {
    this.isDataLoadedService.currentData.subscribe(
      data => (this.isDataLoaded = data)
    );
  }
}
