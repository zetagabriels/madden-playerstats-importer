export default interface IPlayer {
  name: string;
  team: string;
  position: string;
  gamesPlayed: number;
  gamesStarted: number;

  convert(json: any): void;
}
