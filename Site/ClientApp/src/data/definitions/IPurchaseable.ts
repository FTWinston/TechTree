import { ReadonlyDictionary } from '../Dictionary';
import { ISelectable } from './ISelectable';

export interface IPurchasable extends ISelectable {
    buildTime: number;
    prerequisite?: number;
    cost: ReadonlyDictionary<string, number>;
}