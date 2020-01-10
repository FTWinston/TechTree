import React, { FunctionComponent, useMemo } from 'react';
import './Icon.css';

export enum IconStyle {
    Normal = 0,
    Disabled = 1,
    Prerequisite = 2,
}

interface Props {
    symbol: string;
    name: string;
    className?: string;
    style?: IconStyle;
}

export const Icon: FunctionComponent<Props> = props => {
    const classes = useMemo(
        () => {
            let className = props.className === undefined
            ? 'icon'
            : 'icon ' + props.className;

            if (props.style !== undefined) {
                if (props.style & IconStyle.Disabled) {
                    className += ' icon--disabled';
                }
                if (props.style & IconStyle.Prerequisite) {
                    className += ' icon--prerequisite';
                }
            }

            return className;
        },
        [props.style, props.className]
    )

    return (
        <div className={classes} title={props.name}>
            {props.symbol}
        </div>
    );
}