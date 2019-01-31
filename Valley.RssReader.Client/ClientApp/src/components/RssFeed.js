import React, { Component } from 'react';

export class RssFeed extends Component {
  static displayName = RssFeed.name;

  constructor (props) {
    super(props);
    this.state = { rssItems: [], loading: true, currentCount: 0, pageIndex: 0, endOfFeed: false, message: '' };
    this.loadMore = this.loadMore.bind(this);
    this.handleErrors = this.handleErrors.bind(this);
    this.loadMore();
  }

  handleErrors(response) {
    if (!response.ok) {
      this.setState({
        loading: false,
        message: response.statusText
      });
      throw Error(response.statusText);
    } else {
      this.setState({ message: '' });
    }
    return response;
  }

  loadMore() {
    if (!this.state.endOfFeed) {
      fetch('api/RssFeed/GetRssItems/' + this.state.pageIndex + '/' + 5)
        .then(this.handleErrors)
        .then(response => response.json())
        .then(data => {
          this.setState({
            rssItems: this.state.rssItems.concat(data),
            loading: false,
            currentCount: this.state.currentCount + data.length,
            pageIndex: this.state.pageIndex + 1,
            endOfFeed: data.length === 0
          });
        })
        .catch(error => console.log(error));
    }
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
              <td><a href={rssItem.link} target="_blank">Go to article</a></td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render () {
    let contents = this.state.loading ? <p><em>Loading...</em></p> : RssFeed.renderRssFeedTable(this.state.rssItems);
    const loadMore = 'Load More';

    return (
      <div>
        <h1>RSS Feed</h1>
        <p>Current count: <strong>{this.state.currentCount}</strong></p>
        <button className="btn btn-primary" onClick={this.loadMore}>{loadMore}</button>
        {contents}
        <p style={{ color: 'red' }}>{this.state.endOfFeed ? 'No more left to load.' : this.state.message}</p>
        {
          this.state.rssItems.length > 0 ? <button className="btn btn-primary" onClick={this.loadMore}>{loadMore}</button> : null
        }
      </div>
    );
  }
}
