import 'package:ecomonitor/models/auth/register_user_request.dart';
import 'package:ecomonitor/screens/map_screen.dart';
import 'package:ecomonitor/services/auth_service.dart';
import 'package:flutter/material.dart';

class RegisterScreen extends StatefulWidget {
  final AuthService authService;

  const RegisterScreen({
    super.key,
    required this.authService
  });

  @override
  _RegisterScreenState createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  final _formKey = GlobalKey<FormState>();
  final TextEditingController _firstnameController = TextEditingController();
  final TextEditingController _surnameController = TextEditingController();
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();

  bool _isLoading = false;
  String? _error;

  @override
  void dispose(){
    _firstnameController.dispose();
    _surnameController.dispose();
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }


  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() {
      _isLoading = true;
      _error = null;
    });

    try {
      final request = RegisterUserRequest(
        firstname: _firstnameController.text.trim(), 
        surename: _surnameController.text.trim(), 
        email: _emailController.text.trim(), 
        password: _passwordController.text,);

        final token = await widget.authService.registration(request);

        if (token.isNotEmpty) {
          Navigator.pushReplacement(
            context, 
            MaterialPageRoute(builder: (context) => MapScreen()),
            );
        } else {
          setState(() {
            _error = 'Token is empty';
          });
        }
    } catch (e) {
      setState(() {
        _error = e.toString();
      });
    } finally {
      setState(() {
        _isLoading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Registration')),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: Column(
            children: [
              if (_error != null)
              Padding(
                padding: const EdgeInsets.only(bottom: 12),
                child: Text(_error!, style: TextStyle(color: Colors.red)),
                ),
                TextFormField(
                  controller: _firstnameController,
                  decoration: InputDecoration(labelText: 'Firstname'),
                  validator: (value) => value!.isEmpty ? 'Input firstname' : null,
                ),
                TextFormField(
                  controller: _surnameController,
                  decoration: InputDecoration(labelText: 'Surname'),
                  validator: (value) => value!.isEmpty ? 'Input surname' : null,
                ),
                TextFormField(
                  controller: _emailController,
                  decoration: InputDecoration(labelText: 'Email'),
                  validator: (value) => value!.contains('@') ? null : 'Invalid email',
                  keyboardType: TextInputType.emailAddress,
                ),
                TextFormField(
                  controller: _passwordController,
                  decoration: InputDecoration(labelText: 'Password'),
                  obscureText: true,
                  validator: (value) => value!.length < 6 ? 'Less than 6 characters' : null,
                ),
                SizedBox(height: 20),
                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton(
                    onPressed: _isLoading ? null : _submit, 
                    child: _isLoading
                    ? CircularProgressIndicator(color: Colors.white)
                    : Text('Create account'),
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }
}