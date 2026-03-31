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

  markdown: {
    hooks: {
      onBrokenMarkdownLinks: 'warn'
    }
  },

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
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="margin-right:4px;vertical-align:text-bottom;">
              <circle cx="11" cy="11" r="8"/>
              <path d="M21 21l-4.35-4.35"/>
            </svg>
            Search
          `,
          href: '/search',
          position: 'right',
        },
        {
          label: 'GitHub',
          href: 'https://github.com/polatengin/utah',
          position: 'right',
          className: 'navbar-github-link',
        },
        {
          label: 'Issues',
          href: 'https://github.com/polatengin/utah/issues',
          position: 'right',
          className: 'navbar-issues-link',
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
