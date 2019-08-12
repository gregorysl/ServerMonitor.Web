import React, { Component } from "react";
import { Icon, Input } from "antd";

class NoteControl extends Component {
  constructor(props) {
    super(props);
    this.state = {
      value: props.org.note,
      isEditMode: false
    };
    this.confirmNote = this.confirmNote.bind(this);
    this.setEditMode = this.setEditMode.bind(this);
    this.handleKeyPress = this.handleKeyPress.bind(this);
  }

  setEditMode() {
    this.setState({
      isEditMode: !this.state.isEditMode,
      value: this.props.org.note
    });
  }

  updateInputValue(evt) {
    this.setState({
      value: evt.target.value
    });
  }

  handleKeyPress(event) {
    if (event.key === "Enter") {
      this.confirmNote();
    } else if (event.keyCode === 27) {
      this.setEditMode();
    }
  }
  confirmNote() {
    this.setState({ isEditMode: !this.state.isEditMode });
    this.props.org.note = this.state.value;
    this.props.click(
      this.props.org,
      this.props.url,
      this.props.refresh,
      "Note"
    );
  }

  render() {
    const noteProp = this.props.org.note;
    const iconFill = !noteProp || noteProp.length === 0 ? null : "filled";
    const note =
      noteProp &&
      noteProp.split("\\n").map(item => (
        <span key={item}>
          {item}
          <br />
        </span>
      ));
    return this.state.isEditMode ? (
      <div>
        <Input
          className='tag-input'
          placeholder='Note'
          type='text'
          value={this.state.value}
          onPressEnter={this.confirmNote}
          onChange={evt => this.updateInputValue(evt)}
          onKeyDown={this.handleKeyPress}
          prefix={<Icon type='edit' theme={null} />}
        />
        <Icon className='icon-hand' onClick={this.confirmNote} type='check' />
        <Icon className='icon-hand' onClick={this.setEditMode} type='close' />
      </div>
    ) : (
      <div>
        <Icon
          className='icon-hand'
          onClick={this.setEditMode}
          type='edit'
          theme={iconFill}
        />
        {note}
      </div>
    );
  }
}

export default NoteControl;
