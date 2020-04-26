import Player from './player';

export default class PassingPlayer extends Player {
  completions!: number;
  attemptedPasses!: number;
  passingYards!: number;
  passingTouchdowns!: number;
  interceptions!: number;
  firstDowns!: number;
  longestPass!: number;
  sacksTaken!: number;
  fourthQuarterComebacks!: number;

  convert(json: any): void {
    console.log('hello');
    super.convert(json);
    this.completions = json.cmp;
    this.firstDowns = 100;
  }
}
