const esbuild = require("esbuild");

const production = process.argv.includes('--production');
const watch = process.argv.includes('--watch');
const test = process.argv.includes('--test');

/**
 * @type {import('esbuild').Plugin}
 */
const esbuildProblemMatcherPlugin = {
	name: 'esbuild-problem-matcher',

	setup(build) {
		build.onStart(() => {
			console.log('[watch] build started');
		});
		build.onEnd((result) => {
			result.errors.forEach(({ text, location }) => {
				console.error(`✘ [ERROR] ${text}`);
				console.error(`    ${location.file}:${location.line}:${location.column}:`);
			});
			console.log('[watch] build finished');
		});
	},
};

async function main() {
	// Base configuration
	const baseConfig = {
		bundle: true,
		format: 'cjs',
		minify: production,
		sourcemap: !production,
		sourcesContent: false,
		platform: 'node',
		external: ['vscode'],
		logLevel: 'silent',
		plugins: [esbuildProblemMatcherPlugin],
	};

	// Extension build
	const extensionConfig = {
		...baseConfig,
		entryPoints: ['src/extension.ts'],
		outfile: 'dist/extension.js',
	};

	// Test build (only when needed)
	const testConfig = test ? {
		...baseConfig,
		entryPoints: ['src/test/extension.test.ts'],
		outfile: 'dist/test/extension.test.js',
		minify: false, // Don't minify tests
	} : null;

	// Build extension
	const extensionCtx = await esbuild.context(extensionConfig);
	
	// Build tests if needed
	let testCtx;
	if (testConfig) {
		testCtx = await esbuild.context(testConfig);
	}

	if (watch) {
		await extensionCtx.watch();
		if (testCtx) {
			await testCtx.watch();
		}
	} else {
		await extensionCtx.rebuild();
		if (testCtx) {
			await testCtx.rebuild();
		}
		await extensionCtx.dispose();
		if (testCtx) {
			await testCtx.dispose();
		}
	}
}

main().catch(e => {
	console.error(e);
	process.exit(1);
});
