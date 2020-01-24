import { ReadonlyDictionary } from '../Dictionary';

export interface IPurchasable {
    symbol: string;
    name: string;
    prerequisite?: number;
    cost: ReadonlyDictionary<string, number>;
}