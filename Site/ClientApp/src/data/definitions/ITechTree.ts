import { ReadonlyDictionary } from '../Dictionary';
import { ITreeBuilding } from './ITreeBuilding';

export interface ITechTree {
    buildings: ReadonlyDictionary<number, ITreeBuilding>;
}