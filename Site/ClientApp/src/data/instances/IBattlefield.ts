import { ICell } from './ICell';

export interface IBattlefield {
    width: number;
    height: number;
    cells: ICell[];
    startPositions: number[];
}