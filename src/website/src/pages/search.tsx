import React, { useState, useEffect, useCallback, useRef } from 'react';
import Layout from '@theme/Layout';
import Head from '@docusaurus/Head';
import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import BrowserOnly from '@docusaurus/BrowserOnly';
import styles from './search.module.css';

interface SearchResult {
  title: string;
  category: string;
  href: string;
  body: string;
}

function SearchContent() {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState<SearchResult[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const searchFnRef = useRef<((query: string) => Promise<SearchResult[]>) | null>(null);
  const allDocsRef = useRef<SearchResult[]>([]);
  const debounceRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  // Read initial query from URL
  useEffect(() => {
    const params = new URLSearchParams(window.location.search);
    const q = params.get('q');
    if (q) {
      setQuery(q);
    }
  }, []);

  useEffect(() => {
    let cancelled = false;

    async function loadDocfind() {
      try {
        const [mod, docsResp] = await Promise.all([
          // @ts-expect-error — runtime URL, not a bundled module
          import(/* webpackIgnore: true */ '/docfind/docfind.js'),
          fetch('/docfind/documents.json'),
        ]);
        if (cancelled) return;
        await mod.init();
        searchFnRef.current = mod.default;
        allDocsRef.current = await docsResp.json();
        setLoading(false);
      } catch {
        if (cancelled) return;
        setError('Failed to load search index. Please try again later.');
        setLoading(false);
      }
    }

    loadDocfind();
    return () => { cancelled = true; };
  }, []);

  function fallbackSearch(q: string): SearchResult[] {
    const lower = q.toLowerCase();
    const scored: { doc: SearchResult; score: number }[] = [];
    for (const doc of allDocsRef.current) {
      let score = 0;
      if (doc.title.toLowerCase().includes(lower)) score += 10;
      if (doc.category.toLowerCase().includes(lower)) score += 5;
      if (doc.body.toLowerCase().includes(lower)) score += 1;
      if (score > 0) scored.push({ doc, score });
    }
    scored.sort((a, b) => b.score - a.score);
    return scored.map(s => s.doc);
  }

  const runSearch = useCallback(async (q: string) => {
    if (!searchFnRef.current) return;

    if (!q.trim()) {
      setResults([]);
      return;
    }

    try {
      const res = await searchFnRef.current(q);
      if (res.length > 0) {
        setResults(res);
      } else {
        setResults(fallbackSearch(q));
      }
    } catch {
      setResults(fallbackSearch(q));
    }
  }, []);

  useEffect(() => {
    if (loading) return;

    if (debounceRef.current) {
      clearTimeout(debounceRef.current);
    }

    debounceRef.current = setTimeout(() => {
      runSearch(query);

      const url = new URL(window.location.href);
      if (query.trim()) {
        url.searchParams.set('q', query);
      } else {
        url.searchParams.delete('q');
      }
      window.history.replaceState(null, '', url.toString());
    }, 300);

    return () => {
      if (debounceRef.current) {
        clearTimeout(debounceRef.current);
      }
    };
  }, [query, loading, runSearch]);

  useEffect(() => {
    if (!loading && query) {
      runSearch(query);
    }
  }, [loading]);

  return (
    <div className={styles.searchContainer}>
      <div className={styles.inputWrapper}>
        <svg className={styles.searchIcon} width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
          <circle cx="11" cy="11" r="8" />
          <path d="M21 21l-4.35-4.35" />
        </svg>
        <input
          type="text"
          className={styles.searchInput}
          placeholder="Search documentation..."
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          autoFocus
        />
      </div>

      {loading && (<></>)}

      {error && (<div className={styles.statusError}>{error}</div>)}

      {!loading && !error && query.trim() && results.length === 0 && (<div className={styles.resultCount}>No results found for &ldquo;{query}&rdquo;</div>)}

      {results.length > 0 && (
        <div className={styles.results}>
          <p className={styles.resultCount}>
            {results.length} result{results.length !== 1 ? 's' : ''} found
          </p>
          {results.map((result, i) => (
            <Link key={i} to={result.href} className={styles.resultCard}>
              <div className={styles.resultHeader}>
                <span className={styles.resultTitle}>{result.title}</span>
                <span className={styles.resultCategory}>{result.category}</span>
              </div>
              {result.body && (
                <p className={styles.resultBody}>{result.body}</p>
              )}
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}

export default function SearchPage(): React.JSX.Element {
  const { siteConfig } = useDocusaurusContext();

  return (
    <Layout>
      <Head>
        <title>Search | {siteConfig.title}</title>
        <meta name="description" content={`Search ${siteConfig.title} documentation`} />
      </Head>
      <BrowserOnly fallback={<div></div>}>
        {() => <SearchContent />}
      </BrowserOnly>
    </Layout>
  );
}
