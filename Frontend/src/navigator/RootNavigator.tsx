import React from 'react';
import { createStackNavigator } from '@react-navigation/stack';
import MapScreen from '../screens/MapScreen';
import {AllPhotosScreen} from '../screens/AllPhotosScreen';
import {PhotoInfoScreen} from '../screens/PhotoInfoScreen';

const Stack = createStackNavigator();

export const RootNavigator = () => {
    return (
        <Stack.Navigator initialRouteName="Map">
            <Stack.Screen name="Map" component={MapScreen} />
            <Stack.Screen name="AllPhotos" component={AllPhotosScreen} />
            <Stack.Screen name="PhotoInfo" component={PhotoInfoScreen} />
        </Stack.Navigator>
    );
}