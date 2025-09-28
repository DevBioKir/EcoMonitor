import { NavigationContainer } from "@react-navigation/native";
import React from "react";
import DrawerNavigator from './DrawerNavigator';
import { createStackNavigator } from "@react-navigation/stack";
import { PhotoInfoScreen } from "../screens/PhotoInfoScreen";

const Stack = createStackNavigator();

const AppNavigator = () => {
    return(
        <NavigationContainer>
            <Stack.Navigator screenOptions={{ headerShown: true }} >
                <Stack.Screen name="Drawer" component={DrawerNavigator} options={{ headerShown: false }} />
                <Stack.Screen name="PhotoInfo" component={PhotoInfoScreen} />
            </Stack.Navigator>
        </NavigationContainer>
    );
};
export default AppNavigator;