import { themes as prismThemes } from 'prism-react-renderer';
import type { Config } from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';

const config: Config = {
  title: 'Utah',
  tagline: 'Write shell scripts in TypeScript-like syntax',
  favicon: 'assets/favicon.ico',
  url: 'https://utahshx.com',

  future: {
    v4: true,
  },

  baseUrl: '/',
  organizationName: 'polatengin',
  projectName: 'utah',
  trailingSlash: false,

  onBrokenLinks: 'warn',
  onBrokenMarkdownLinks: 'warn',

  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  presets: [
    [
      'classic',
      {
        docs: {
          path: '../../docs',
          routeBasePath: '/',
          sidebarItemsGenerator: async function ({
            defaultSidebarItemsGenerator,
            ...args
          }) {
            return defaultSidebarItemsGenerator(args);
          },
          editUrl:
            'https://github.com/polatengin/utah/tree/main/src/website/',
        },
        blog: false,
        theme: {
          customCss: './static/assets/custom.css',
        },
      } satisfies Preset.Options,
    ],
  ],

  themeConfig: {
    colorMode: {
      defaultMode: 'dark',
      respectPrefersColorScheme: true,
    },
    navbar: {
      title: 'Utah',
      logo: {
        alt: 'Utah Logo',
        src: 'assets/logo.svg',
      },
      items: [
        {
          label: 'Getting Started',
          href: '/getting-started',
          position: 'left',
        },
        {
          label: 'Language Features',
          href: '/language-features',
          position: 'left',
        },
        {
          label: 'CLI Reference',
          href: '/cli',
          position: 'left',
        },
        {
          label: 'Guides',
          href: '/guides',
          position: 'left',
        },
        {
          label: 'Examples',
          href: '/examples',
          position: 'left',
        },
        {
          html: `
            <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor" style="margin-right:4px;vertical-align:text-bottom;">
              <path d="M12 0c-6.626 0-12 5.373-12 12 0 5.302 3.438 9.8 8.207 11.387.599.111.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v3.293c0 .319.192.694.801.576 4.765-1.589 8.199-6.086 8.199-11.386 0-6.627-5.373-12-12-12z"/>
            </svg>
            GitHub
          `,
          href: 'https://github.com/polatengin/utah',
          position: 'right',
        },
        {
          label: 'Issues',
          href: 'https://github.com/polatengin/utah/issues',
          position: 'right',
        },
        {
          label: 'Discussions',
          href: 'https://github.com/polatengin/utah/discussions',
          position: 'right',
        },
      ],
    },
    prism: {
      theme: prismThemes.github,
      darkTheme: prismThemes.dracula,
    },
  } satisfies Preset.ThemeConfig,
};

export default config;
