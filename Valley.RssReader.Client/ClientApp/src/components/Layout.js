import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';

import backgroundImage from '../assets/background.png';
var backgroundImageStyle = {
  width: "100vw",
  minHeight: "100vh",
  backgroundImage: `url(${backgroundImage})`,
  backgroundSize: 'cover'
};

export class Layout extends Component {
  static displayName = Layout.name;

  render () {
    return (
      <div style={backgroundImageStyle}>
        <NavMenu />
        <Container>
          {this.props.children}
        </Container>
      </div>
    );
  }
}
