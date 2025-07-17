import { Component } from "react";

export class AllPhotosScreen extends Component {
    state = {
        list: [],
        isLoading: false,
    };

    componentsDidMount = () => {
        this.onRefresh();
    };

    getMoreData = (isRefreshing) => {};

    onRefresh = () => {
        this.getMoreData(true);
    };

    jnScrollToEnd = ({distanceFromEnd}) => {
        if (distanceFromEnd < 0) {
            return;
        }
        this.getMoreData(false);
    };

    onItemPress = (item) => {
        this.props.navigation.navigate('info', {person: item});
    };

    keyExtractor = (person) => person.login.uuid;

    renderItem = ({item}) => {
        return (
            <></>
        );
    }
}