import IPlayer from './iplayer';

export default abstract class Player implements IPlayer {
  name!: string;
  team!: string;
  position!: string;
  gamesPlayed!: number;
  gamesStarted!: number;

  convert(json: any): void {
    this.name = json.player;
    this.team = json.team;
    this.position = json.pos;
    this.gamesPlayed = json.g;
    this.gamesStarted = json.gs;
  }
}
