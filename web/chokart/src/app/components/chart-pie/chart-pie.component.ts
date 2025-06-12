import { Component, Input, SimpleChanges } from '@angular/core';
import { ChartModule } from 'primeng/chart';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-chart-pie',
  standalone: true,
  imports: [ChartModule],
  templateUrl: './chart-pie.component.html',
  styleUrl: './chart-pie.component.css',
})
export class ChartPieComponent {
  data: any;
  options: any;

  constructor(private cd: ChangeDetectorRef) {}
  @Input() characterUsageStats: { [characterId: number]: number } = {};
  @Input() characterMap: { [id: number]: string } = {};

  ngOnInit() {
    this.initChart();
  }

  oldData = this.characterUsageStats;

  ngOnChanges(changes: SimpleChanges): void {
    if (
      this.characterUsageStats &&
      Object.keys(this.characterUsageStats).length > 0 &&
      this.characterMap &&
      Object.keys(this.characterMap).length > 0
    ) {
      this.initChart();
    }
  }

  initChart() {
    const usage = this.characterUsageStats;
    const map = this.characterMap;

    const labels = Object.keys(usage).map((id) => map[+id] || `ID ${id}`);
    const dataValues = Object.values(usage);

    this.data = {
      labels,
      datasets: [
        {
          data: dataValues,
          backgroundColor: [
            '#41a5ee',
            '#ffb718',
            '#ff6d4c',
            '#484848',
            '#87c600',
            '#d07ae9',
          ],
          hoverBackgroundColor: [
            '#5dbaff',
            '#ffe24c',
            '#fb8c73',
            '#666666',
            '#bdd84f',
            '#e69bfc',
          ],
        },
      ],
    };

    this.options = {
      plugins: {
        legend: {
          labels: {
            usePointStyle: true,
            color: '#ffffff',
          },
        },
      },
    };
  }
}
