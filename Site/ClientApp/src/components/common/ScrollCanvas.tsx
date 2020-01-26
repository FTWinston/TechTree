import React, { useMemo, useRef, useEffect, forwardRef, useState, CSSProperties } from 'react';
import './ScrollCanvas.css';

interface Props {
    className?: string;
    contentWidth: number;
    contentHeight: number;
    boundsChanged: (bounds: DOMRect) => void;
    onClick?: (x: number, y: number) => void;
}

export const ScrollCanvas = forwardRef<HTMLCanvasElement, Props>((props, ref) => {
    const outerRef = useRef<HTMLDivElement>(null);
    const scrollRef = useRef<HTMLDivElement>(null);
    let canvasRef = useRef<HTMLCanvasElement>(null);

    if (ref) {
        canvasRef = ref as React.RefObject<HTMLCanvasElement>;
    }

    let onClick = useMemo(
        () => {
            const { onClick } = props;
            if (!onClick || !outerRef.current) {
                return undefined;
            }

            const outer = outerRef.current;

            return (e: React.MouseEvent<HTMLDivElement>) => {
                const { x, y } = resolvePosition(e, outer);

                onClick(x, y);
            }
        },
        [props.onClick]
    );
    if (!props.onClick) {
        onClick = undefined;
    }

    const scrollStyle = useMemo(
        () => ({
            width: `${props.contentWidth}px`,
            height: `${props.contentHeight}px`
        }),
        [props.contentWidth, props.contentHeight]
    );

    const scrollSize = useMemo(
        () => getScrollbarSize(),
        [],
    );

    const [displaySizeStyle, setDisplaySizeStyle] = useState<CSSProperties>();

    useEffect(
        () => {
            const display = canvasRef.current;
            const scroll = scrollRef.current;

            if (!display || !scroll) {
                return;
            }

            const updateCanvasSize = () => {
                const displayWidth = outerRef.current!.clientWidth - scrollSize.width;
                const displayHeight = outerRef.current!.clientHeight - scrollSize.height;

                if (display.width !== displayWidth || display.height !== displayHeight) {
                    display.width = displayWidth;
                    display.height = displayHeight;

                    setDisplaySizeStyle({
                        width: `${displayWidth}px`,
                        height: `${displayHeight}px`,
                    });

                    const bounds = new DOMRect(
                        scroll.scrollLeft,
                        scroll.scrollTop,
                        displayWidth,
                        displayHeight
                    );
                    
                    // TODO: ensure this isn't too early in the lifecycle. Has display size update applied?
                    props.boundsChanged(bounds);
                }
            };

            updateCanvasSize();

            window.addEventListener('resize', updateCanvasSize);

            return () => window.removeEventListener('resize', updateCanvasSize);
        },
        [canvasRef.current, props.boundsChanged]
    );

    const onScroll = useMemo(
        () => (e: React.UIEvent<HTMLDivElement>) => {
            const bounds = new DOMRect(
                e.currentTarget.scrollLeft,
                e.currentTarget.scrollTop,
                canvasRef.current!.width,
                canvasRef.current!.height
            );

            props.boundsChanged(bounds);
        },
        [canvasRef.current, props.boundsChanged]
    );

    const classes = props.className
        ? `scrollCanvas ${props.className}`
        : 'scrollCanvas';

    return (
        <div className={classes} ref={outerRef}>
            <canvas
                className="scrollCanvas__display"
                style={displaySizeStyle}
                ref={ref}
            />
            <div
                className="scrollCanvas__scroll"
                style={displaySizeStyle}
                onClick={onClick}
                onScroll={onScroll}
                ref={scrollRef}
            >
                <div
                    className="scrollCanvas__size"
                    style={scrollStyle}
                />
            </div>
        </div>
    );
})

function resolvePosition(e: React.MouseEvent<HTMLDivElement, MouseEvent>, outer: HTMLDivElement) {
    const scroller = e.currentTarget;
    const outerBounds = outer.getBoundingClientRect();
    
    return {
        x: e.clientX - outerBounds.left + scroller.scrollLeft,
        y: e.clientY - outerBounds.top + scroller.scrollTop,
    };
}

function getScrollbarSize() {
    let outer = document.createElement('div');
    outer.style.visibility = 'hidden';
    outer.style.width = '100px';
    outer.style.height = '100px';

    document.body.appendChild(outer);

    let widthNoScroll = outer.offsetWidth;
    let heightNoScroll = outer.offsetHeight;

    outer.style.overflow = 'scroll';

    let inner = document.createElement('div');
    inner.style.width = '100%';
    inner.style.height = '100%';
    outer.appendChild(inner);

    let widthWithScroll = inner.offsetWidth;
    let heightWithScroll = inner.offsetHeight;

    (outer.parentNode as HTMLElement).removeChild(outer);

    return {
        width: widthNoScroll - widthWithScroll,
        height: heightNoScroll - heightWithScroll,
    };
}