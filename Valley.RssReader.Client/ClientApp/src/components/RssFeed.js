import React, { Component } from 'react';

export class RssFeed extends Component {
  static displayName = RssFeed.name;

  constructor (props) {
    super(props);
    this.state = { rssItems: [], loading: true, currentCount: 0 };
    this.loadMore = this.loadMore.bind(this);
    this.loadMore();
  }

  loadMore () {
    fetch('api/RssFeed/GetRssItems/' + (this.state.currentCount / 5) + '/' + 5)
      .then(response => response.json())
      .then(data => {
        this.setState({ rssItems: this.state.rssItems.concat(data), loading: false, currentCount: this.state.currentCount + data.length });
      });
  }

  static renderRssFeedTable (rssItems) {
    return (
      <table className='table table-striped'>
        <thead>
          <tr>
            <th>Title</th>
            <th>Description</th>
            <th>Categories</th>
            <th>Date</th>
            <th>Link</th>
          </tr>
        </thead>
        <tbody>
          {rssItems.map(rssItem =>
            <tr key={rssItem.Id}>
              <td>{rssItem.title}</td>
              <td>{rssItem.description}</td>
              <td>{rssItem.categories}</td>
              <td>{rssItem.date}</td>
              <td>{rssItem.link}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render () {
    let contents = this.state.loading ? <p><em>Loading...</em></p> : RssFeed.renderRssFeedTable(this.state.rssItems);

    return (
      <div>
        <h1>RSS Feed</h1>
        <p>Current count: <strong>{this.state.currentCount}</strong></p>
        {contents}
        <button className="btn btn-primary" onClick={this.loadMore}>Load More</button>
      </div>
    );
  }
}
