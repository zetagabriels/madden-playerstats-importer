import Player from './player';

export default class ReceivingPlayer extends Player {
  receptions!: number;
  yardsReceived!: number;
  touchdowns!: number;
  firstDowns!: number;
  longestReception!: number;
  fumbles!: number;

  public convert(json: any): void {
    super.convert(json);
    this.receptions = json.rec || 0;
    this.yardsReceived = json.rec_yds || 0;
    this.touchdowns = json.rec_td || 0;
    this.firstDowns = json.rec_first_down || 0;
    this.longestReception = json.rec_long || 0;
    this.fumbles = json.fumbles || 0;
  }
}
