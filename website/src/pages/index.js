import React from "react";
import clsx from "clsx";
import Link from "@docusaurus/Link";
import useBaseUrl, { useBaseUrlUtils } from "@docusaurus/useBaseUrl";
import useDocusaurusContext from "@docusaurus/useDocusaurusContext";
import Layout from "@theme/Layout";
import LiteYouTubeEmbed from "react-lite-youtube-embed";

import styles from "./index.module.css";
import "react-lite-youtube-embed/dist/LiteYouTubeEmbed.css";

function HomepageHeader() {
  const { siteConfig } = useDocusaurusContext();
  return (
    <header className={clsx("hero", styles.heroBanner)}>
      <div className="container">
        <img
          alt={siteConfig.title}
          src={useBaseUrl("/img/logo.svg")}
          width="512"
        />
        <p className="hero__subtitle">{siteConfig.tagline}</p>
        <div className={styles.buttons}>
          <Link className="button button--secondary button--lg" to="/docs/">
            Get Started
          </Link>
        </div>
      </div>
    </header>
  );
}

function VideoContainer() {
  return (
    <div className="container text--center margin-bottom--xl">
      <div className="row">
        <div className="col">
          <h2>Check it out in a intro video</h2>
          <div className={styles.videoContainer}>
            <LiteYouTubeEmbed
              id="paYrVKO1Pi4"
              params="autoplay=1&autohide=1&showinfo=0&rel=0"
              title="The NETFLIX Way to Keep Your .NET APIs Reliable"
              poster="maxresdefault"
              webp
            />
          </div>
        </div>
      </div>
    </div>
  );
}

export default function Home() {
  const { siteConfig } = useDocusaurusContext();
  return (
    <Layout
      title={`Hello from ${siteConfig.title}`}
      description="Description will go into a meta tag in <head />"
    >
      <HomepageHeader />
      <main>
        <VideoContainer />
      </main>
    </Layout>
  );
}
