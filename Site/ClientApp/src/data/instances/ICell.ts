import { IBuilding } from './IBuilding';
import { IUnit } from './IUnit';

export interface ICell {
    type: CellType;
    visiblity: Visibility;
    content?: IBuilding | IUnit;
}

export enum Visibility {
    Unseen = 0,
    Seen = 1,
    Visible = 2,
}

export enum CellType {
    Unknown = 0,
    Water = 1,
    Flat = 2,
    Difficult = 3,
    Unpassable = 4,
}