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
          label: 'CLI Reference',
          href: '/cli',
          position: 'left',
        },
        {
          label: 'Language Features',
          href: '/language-features',
          position: 'left',
        },
        {
          label: 'GitHub',
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
