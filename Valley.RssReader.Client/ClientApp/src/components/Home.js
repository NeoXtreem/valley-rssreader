import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <h1>The Valley RSS Reader</h1>
        <p>Welcome to The Valley where RSS feeds happen!</p>
      </div>
    );
  }
}
