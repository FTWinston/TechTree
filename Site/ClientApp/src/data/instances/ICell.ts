export interface ICell {
    type: number;
}

export enum CellType {
    OutOfBounds = 0,
    Water = 1,
    Flat = 2,
    Difficult = 3,
    Unpassable = 4,
}