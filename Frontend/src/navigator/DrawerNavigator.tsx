import { createDrawerNavigator } from '@react-navigation/drawer';
import MapScreen from '../screens/MapScreen';
import { AllPhotosScreen } from '../screens/AllPhotosScreen';

const Drawer = createDrawerNavigator();

const AppNavigator = () => {
  return (
    <Drawer.Navigator initialRouteName="Map">
      <Drawer.Screen name="Map" component={MapScreen} />
      <Drawer.Screen name="AllPhotos" component={AllPhotosScreen} />
    </Drawer.Navigator>
  );
};
export default AppNavigator;
