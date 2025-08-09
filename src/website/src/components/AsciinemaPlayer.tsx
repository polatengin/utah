import React, { useEffect, useRef } from 'react';
import 'asciinema-player/dist/bundle/asciinema-player.css';

interface AsciinemaPlayerProps {
  src: string;
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

const AsciinemaPlayer: React.FC<AsciinemaPlayerProps> = ({
  src,
  cols = 120,
  rows = 30,
  autoPlay = false,
  preload = true,
  loop = false,
  startAt,
  speed = 1,
  idleTimeLimit = 2,
  theme = 'asciinema',
  poster,
  fit = 'width',
  fontSize = '15px',
}) => {
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const loadPlayer = async () => {
      // Dynamically import asciinema-player to avoid SSR issues
      const { create } = await import('asciinema-player');
      
      if (ref.current) {
        // Clear any existing content
        ref.current.innerHTML = '';
        
        // Create the player
        create(src, ref.current, {
          cols,
          rows,
          autoPlay,
          preload,
          loop,
          startAt,
          speed,
          idleTimeLimit,
          theme,
          poster,
          fit,
          fontSize,
        });
      }
    };

    loadPlayer();
  }, [src, cols, rows, autoPlay, preload, loop, startAt, speed, idleTimeLimit, theme, poster, fit, fontSize]);

  return (
    <div 
      ref={ref} 
      style={{ 
        margin: '20px 0',
        borderRadius: '8px',
        overflow: 'hidden',
        boxShadow: '0 4px 12px rgba(0, 0, 0, 0.15)',
        background: '#0d1117'
      }}
    />
  );
};

export default AsciinemaPlayer;
