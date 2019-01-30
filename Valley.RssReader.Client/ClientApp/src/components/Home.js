import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <h1>The Valley RSS Reader</h1>
        <p>Welcome to The Valley where RSS feeds happen!</p>
        <p>Navigate to the RSS feed in the top navigation bar, or just <a href="/rss-feed">click here</a> to go there directly.</p>
      </div>
    );
  }
}
