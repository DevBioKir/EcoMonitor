import React from 'react';
import { createStackNavigator } from '@react-navigation/stack';
import MapScreen from '../screens/MapScreen';

const Stack = createStackNavigator();

export const RootNavigator = () => {
    return (
        <Stack.Navigator initialRouteName="Map">
            <Stack.Screen name="Map" component={MapScreen} />
            {/*<Stack.Screen name="AllPhotos" component={AllPhotos} />*/}
        </Stack.Navigator>
    );
}