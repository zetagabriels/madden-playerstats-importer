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

  public static convert(json: any): PassingPlayer {
    const p = super.convert(json) as PassingPlayer;
    return p;
  }
}