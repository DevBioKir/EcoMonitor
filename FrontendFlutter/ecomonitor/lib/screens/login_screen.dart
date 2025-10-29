import 'package:ecomonitor/screens/map_screen.dart';
import 'package:ecomonitor/screens/register_screen.dart';
import 'package:ecomonitor/services/auth_service.dart';
import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';

class LoginScreen extends StatefulWidget{
  //final Future<void> Function(String login, String password) onLogin;
  final VoidCallback onRegister;
  final AuthService authService;

  const LoginScreen({
    super.key,
    required this.onRegister,
    required this.authService});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _formKey = GlobalKey<FormState>();
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  bool _obscurePassword = true;
  bool _loading = false;
  String? _error;

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }
  
  Future<bool> _handleLogin() async {
    if(!_formKey.currentState!.validate()) return false;
    setState(() {
      _loading = true;
      _error = null;
    });
    try{
      final loginResponse = await widget.authService.login(
      _emailController.text.trim(),
      _passwordController.text.trim(),
      );

      if (loginResponse.accessToken.isNotEmpty) {
        Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (context) => 
                      MapScreen())
          );
          return true;
      } else {
        throw Exception('Token is empty');
      } 
    } catch (e) {
      setState(() {
        _error = e.toString();
      });
      return false;
    } finally {
      setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Entrance')),
      body: Center(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(24),
          child: Form(
            key: _formKey,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                if (_error != null)
                Padding(
                  padding: const EdgeInsets.only(bottom: 12),
                  child: Text(_error!, style: const TextStyle(color: Colors.red)),
                  ),
                  TextFormField(
                    controller: _emailController,
                    enabled: !_loading,
                    decoration: const InputDecoration(
                      labelText: 'Login',
                      border: OutlineInputBorder(),
                    ),
                    validator: (value) => 
                    value?.isEmpty ?? true ? 'Input Login' : null,
                  ),
                  const SizedBox(height: 16),
                  TextFormField(
                    controller: _passwordController,
                    enabled: !_loading,
                    obscureText: _obscurePassword,
                    decoration: InputDecoration(
                      labelText: 'Password',
                      border: const OutlineInputBorder(),
                      suffixIcon: IconButton(
                        icon: Icon(
                          _obscurePassword ? Icons.visibility : Icons.visibility_off,
                        ),
                        onPressed: () => setState(() {
                          _obscurePassword = !_obscurePassword;
                        }),
                      ),
                    ),
                    validator: (value) => 
                    value?.isEmpty ?? true ? 'Input password' : null,
                  ),
                const SizedBox(height: 24),
                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton(
                    onPressed: _loading ? null : _handleLogin,
                    child: _loading
                        ? const SizedBox(
                            width: 24, height: 24,
                            child: CircularProgressIndicator(strokeWidth: 2),
                          )
                        : const Text('Log in'),
                  ),
                ),
                const SizedBox(height: 16),
                SizedBox(
                  width: double.infinity,
                  child: OutlinedButton(
                    onPressed: _loading ? null : () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => RegisterScreen(
                            authService: widget.authService),
                        ),
                      );
                    },
                    child: const Text("Don't have an account? Sign up"),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}