import React, { FunctionComponent, useMemo } from 'react';
import './Icon.css';

interface Props {
    symbol: string;
    name: string;
    className?: string;
}

export const Icon: FunctionComponent<Props> = props => {
    const classes = props.className === undefined
        ? 'icon'
        : 'icon ' + props.className;

    return (
        <div className={classes} title={props.name}>
            {props.symbol}
        </div>
    );
}