export type Dictionary<K extends keyof any, T> = Partial<Record<K, T>>

export type ReadonlyDictionary<K extends keyof any, T> = Readonly<Partial<Record<K, Readonly<T>>>>