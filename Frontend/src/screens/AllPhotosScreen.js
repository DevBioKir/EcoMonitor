import { Component } from "react";
import { FlatList, Text, TouchableOpacity, View } from 'react-native';
import { getAllPhotos } from "../services/GetAllBinPhotos"

export class AllPhotosScreen extends Component {
    state = {
        list: [],
        isLoading: false,
    };

    componentDidMount = () => {
        this.onRefresh();
    };

    getMoreData = (isRefreshing) => {};

    onRefresh = async() => {
        this.setState({isLoading: true});
        try{
            const date = await getAllPhotos();
            this.setState({list: date})
        } catch (err) {
            console.error(err);
        } finally {
            this.setState({isLoading: false})
        }
    };

    jnScrollToEnd = ({distanceFromEnd}) => {
        if (distanceFromEnd < 0) {
            return;
        }
        this.getMoreData(false);
    };

    onItemPress = (item) => {
        this.props.navigation.navigate('PhotoInfo', {photo: item});
    };

    keyExtractor = (item) => item.id || item.url || String(item.someUniqueId);

    renderItem = ({ item }) => (
    <TouchableOpacity onPress={() => this.onItemPress(item)}>
      <View style={{ padding: 10, borderBottomWidth: 1, borderColor: '#ddd' }}>
        <Text>{item.comment || 'Без комментария'}</Text>
        {/* можно добавить превью фото, дату и т.п. */}
      </View>
    </TouchableOpacity>
  );

  render() {
    return (
      <FlatList
        data={this.state.list}
        keyExtractor={this.keyExtractor}
        renderItem={this.renderItem}
        refreshing={this.state.isLoading}
        onRefresh={this.onRefresh}
      />
    );
  }
}