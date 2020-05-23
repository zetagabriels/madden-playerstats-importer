import Player from './player';

export default class RushingPlayer extends Player {
  rushAttempts!: number;
  yards!: number;
  rushTouchdowns!: number;
  firstDowns!: number;
  longestRush!: number;
  fumbles!: number;

  public convert(json: any): void {
    super.convert(json);
    this.rushAttempts = json.rush_att || 0;
    this.yards = json.rush_yds || 0;
    this.firstDowns = json.rush_first_down || 0;
    this.longestRush = json.rush_long || 0;
    this.fumbles = json.fumbles || 0;
  }
}
