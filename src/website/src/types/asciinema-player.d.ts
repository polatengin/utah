declare module 'asciinema-player' {
  export interface PlayerOptions {
    cols?: number;
    rows?: number;
    autoPlay?: boolean;
    preload?: boolean;
    loop?: boolean | number;
    startAt?: number | string;
    speed?: number;
    idleTimeLimit?: number;
    theme?: string;
    poster?: string;
    fit?: string;
    fontSize?: string;
  }

  export function create(
    src: string,
    element: HTMLElement,
    options?: PlayerOptions
  ): void;
}

declare module 'asciinema-player/dist/bundle/asciinema-player.css';
